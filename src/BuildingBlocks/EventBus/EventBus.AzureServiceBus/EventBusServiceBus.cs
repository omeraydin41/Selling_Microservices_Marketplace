using EventBus.Base.Events;
using EventBus.Base.SubManager;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.AzureServiceBus
{
    public class EventBusServiceBus : BaseEventBus // BaseEventBus'tan türeyen Azure Service Bus event sistemi
    {
        private ITopicClient topicClient; // Mesaj göndermek için kullanılan client
        private ManagementClient managementClient; // Topic ve Subscription yönetim işlemleri için kullanılan client
        private ILogger logger; // Loglama sistemi

        public EventBusServiceBus(EventBusConfig config, IServiceProvider serviceProvider) : base(config, serviceProvider)
        {
            logger = serviceProvider.GetService(typeof(ILogger<EventBusServiceBus>)) as ILogger<EventBusServiceBus>; // Logger sınıfını DI içinden alıyoruz
            managementClient = new ManagementClient(config.EventBusConnectionString); // Azure yönetim client'ını oluşturuyoruz
            topicClient = createTopicClient(); // TopicClient oluşturup mesaj göndermeye hazırlıyoruz
        }

        private ITopicClient createTopicClient()
        {
            if (topicClient != null || !topicClient.IsClosedOrClosing) // Eğer client zaten varsa veya kapalı değilse
            {
                topicClient = new TopicClient(EventBusConfig.EventBusConnectionString, EventBusConfig.DefaultTopicName, RetryPolicy.Default); // Yeni topic client oluştur
            }

            if (!managementClient.TopicExistsAsync(EventBusConfig.DefaultTopicName).GetAwaiter().GetResult()) // Topic yoksa kontrol et
                managementClient.CreateTopicAsync(EventBusConfig.DefaultTopicName).GetAwaiter().GetResult(); // Topic oluştur

            return topicClient; // Topic client geri döner
        }

        public override void Publish(IntegrationEvent @event) // Event'i Azure Service Bus'a gönderen fonksiyon
        {
            var eventName = @event.GetType().Name; // Event'in sınıf adını alıyoruz (ör: OrderCreatedEvent)
            eventName = ProcessEventName(eventName); // Event adını formatlıyoruz

            var eventStr = JsonConvert.SerializeObject(@event); // Event'i JSON string'e dönüştürüyoruz
            var bodyArr = Encoding.UTF8.GetBytes(eventStr); // JSON'u byte dizisine çeviriyoruz (Azure böyle istiyor)

            var message = new Message() // Azure Service Bus mesaj objesi oluştur
            {
                MessageId = Guid.NewGuid().ToString(), // Her mesaj için benzersiz ID oluştur
                Body = bodyArr, // Mesajın içeriği
                Label = eventName // Mesajın tür etiketi (hangi event olduğunu belirtiyor)
            };

            topicClient.SendAsync(message).GetAwaiter().GetResult(); // Mesajı Azure'a gönder
        }

        public override void Subscribe<T, TH>() // Bir event'e abone olmak için kullanılır
        {
            var eventName = typeof(T).Name; // Event adını al (sınıf adı)
            eventName = ProcessEventName(eventName); // Event adını düzenle

            if (!subsManager.HasSubscriptionsForEvent(eventName)) // Bu event'e daha önce abone olunmuş mu?
            {
                var subscriptionClient = createSubClientIfNotExist(eventName); // Yoksa subscription oluştur
                RegisterSubscriptionClientMessageHandler(subscriptionClient); // Mesaj gelince tetiklenecek handler'ı bağla
            }

            logger.LogInformation("Subscription to event {eventName} with {EventHandler}", eventName, typeof(TH).Name); // Log at
            subsManager.AddSubscription<T, TH>(); // Aboneliği belleğe ekle
        }

        public override void UnSubscribe<T, TH>() // Event aboneliğini kaldırmak için
        {
            var eventName = typeof(T).Name; // Event adını al
            try
            {
                var subscirptionClient = CreateSubscriptionClient(eventName); // Subscription client oluştur
                subscirptionClient.RemoveRuleAsync(eventName).GetAwaiter().GetResult(); // Event için rule'u sil
            }
            catch (MessagingEntityNotFoundException) // Rule yoksa
            {
                logger.LogWarning(" the messaging entity {eventName} Could not be found", eventName); // Uyarı ver
            }

            logger.LogWarning("Unsubscripbing  from event {eventName}  ", eventName); // Log
            subsManager.RemoveSubscription<T, TH>(); // Bellekten aboneliği sil
        }

        private void RegisterSubscriptionClientMessageHandler(ISubscriptionClient subscriptionClient) // Gelen mesajları işleyen fonksiyon
        {
            subscriptionClient.RegisterMessageHandler( // Azure’dan mesaj geldikçe tetiklenecek fonksiyon
                async (message, token) =>
                {
                    var eventName = $"{message.Label}"; // Mesajın etiketi = event adı
                    var messageData = Encoding.UTF8.GetString(message.Body); // Mesaj içeriğini string'e çevir

                    if (await ProcessEvent(eventName, messageData)) // Event'i işlediysek
                    {
                        await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken); // Mesajı tamamla (tekrar gelmesin)
                    }
                },
                new MessageHandlerOptions(ExceptionReceivedHandler) // Hata olursa tetiklenecek handler
                {
                    MaxConcurrentCalls = 10, // Aynı anda 10 mesaj işlenebilir
                    AutoComplete = false // Mesaj otomatik tamamlanmaz (biz tamamlıyoruz)
                });
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs) // Hata yönetimi
        {
            var ex = exceptionReceivedEventArgs.Exception; // Hatanın kendisi
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext; // Hata bağlam bilgisi

            logger.LogError(ex, "ERROR handling message: ({ExceptionMessage}) - Context: ({ExceptionContext})", ex.Message, context); // Hata logla

            return Task.CompletedTask; // Async dönüş
        }

        private ISubscriptionClient createSubClientIfNotExist(string eventName) // Event için lazım olan subscription yoksa oluşturur
        {
            var subClient = CreateSubscriptionClient(eventName); // Subscription client yarat
            var exist = managementClient.SubscriptionExistsAsync(
                EventBusConfig.DefaultTopicName,
                getSubName(eventName)
            ).GetAwaiter().GetResult(); // Subscription var mı bak

            if (!exist) // Yoksa
            {
                managementClient.CreateSubscriptionAsync( // Yeni subscription oluştur
                    EventBusConfig.DefaultTopicName,
                    getSubName(eventName)
                ).GetAwaiter().GetResult();

                RemoveDefaultRule(subClient); // Default rule'u sil (yoksa tüm mesajları alır)
            }

            CreateRuleIfNotExists(ProcessEventName(eventName), subClient); // Bu event için rule oluştur

            return subClient; // Subscription client geri döner
        }

        private void CreateRuleIfNotExists(string eventName, ISubscriptionClient subscriptionClient) // Event ile subscription arasında filtreleme kuralı ekler
        {
            bool ruleExists;

            try
            {
                var rule = managementClient.GetRuleAsync(
                    EventBusConfig.DefaultTopicName,
                    getSubName( eventName),
                    eventName
                ).GetAwaiter().GetResult(); // Rule var mı kontrol et
                ruleExists = rule != null; // Varsa true
            }
            catch (MessagingEntityNotFoundException) // Rule yoksa buraya düşer
            {
                ruleExists = false;
            }

            if (!ruleExists) // Rule yoksa
            {
                subscriptionClient.AddRuleAsync(new RuleDescription // Yeni rule ekle
                {
                    Filter = new CorrelationFilter { Label = eventName }, // Sadece bu event label'ına sahip mesajlar gelsin
                    Name = eventName // Rule adı event adı olsun
                }).GetAwaiter().GetResult();
            }
        }

        private void RemoveDefaultRule(SubscriptionClient subscriptionClient) // Azure subscription oluşturunca otomatik gelen default rule'u siler
        {
            try
            {
                subscriptionClient
                    .RemoveRuleAsync(RuleDescription.DefaultRuleName) // $Default rule'unu sil
                    .GetAwaiter()
                    .GetResult();
            }
            catch (MessagingEntityNotFoundException) // Rule yoksa
            {
                logger.LogWarning("The messaging entity ({DefaultRuleName}) could not be found.", RuleDescription.DefaultRuleName);
            }
        }

        private SubscriptionClient CreateSubscriptionClient(string eventName) // Subscription client oluşturur
        {
            return new SubscriptionClient(
                EventBusConfig.EventBusConnectionString, // Azure connection string
                EventBusConfig.DefaultTopicName, // Topic adı
                getSubName(eventName) // Subscription adı (genelde event ismine göre)
            );
        }

        public override void Dispose() // Nesneyi kapatmak için
        {
            topicClient.CloseAsync().GetAwaiter().GetResult(); // Topic client kapat
            managementClient.CloseAsync().GetAwaiter().GetResult(); // Management client kapat
            base.Dispose(); // Base class dispose

            topicClient = null; // Temizle
            managementClient = null; // Temizle
        }
    }



}

