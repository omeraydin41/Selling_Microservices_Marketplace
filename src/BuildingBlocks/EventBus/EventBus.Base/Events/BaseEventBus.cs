using EventBus.Base.Abstraction;
using EventBus.Base.SubManager;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base.Events
{
    public abstract class BaseEventBus : IEventBus//IEVENT BUS interfacesini uyguladık 
    {
        public readonly IServiceProvider serviceProvider;//servis sağlayıcı
        public readonly IEventBusSubscriptionManager subsManager;//abonelik yöneticisi
        public readonly IServiceProvider serviceProvider; // DI konteyner – servisleri oluşturmak için kullanılır
        public readonly IEventBusSubscriptionManager subsManager; // Event aboneliklerini tutan ve yöneten sınıf

        private EventBusConfig eventBusConfig;//event bus konfigürasyonu
        public EventBusConfig EventBusConfig { get; set; } // Event Bus ayarlarını tutan config sınıfı

        public BaseEventBus(EventBusConfig config, IServiceProvider serviceProvider)
        {
            EventBusConfig = config; // Config değerlerini property'e atıyoruz
            serviceProvider = serviceProvider; // DI servis sağlayıcısını class içine atıyoruz (BU SATIR HATALI çünkü kendine atıyorsun)
            subsManager = new InMemoryEventBusSubscriptionManager(ProcessEventName); // Abonelik yöneticisini oluşturuyoruz
            eventBusConfig = config;//event bus konfigürasyonunu atar
            serviceProvider = serviceProvider;//servis sağlayıcıyı atar
            subsManager = new InMemoryEventBusSubscriptionManager(ProcessEventName);//abonelik yöneticisini oluşturur
        }

       

        // Event adını düzenleyen (prefix/suffix kırpan) metod
        public virtual string ProcessEventName(string eventName)
        {
            if (eventBusConfig.DeleteEventPrefix)//eğer event adı ön eki silinecekse
            if (EventBusConfig.DeleteEventPrefix) // Eğer event adı başındaki prefix silinsin ayarı açıksa
            {
                eventName = eventName.TrimStart(eventBusConfig.EventNamePrefix.ToArray());
                eventName = eventName.TrimStart(EventBusConfig.EventNamePrefix.ToArray()); // Prefix'i sil
            }
            if (eventBusConfig.DeleteEventSuffix)//eğer event adı son eki silinecekse
            if (EventBusConfig.DeleteEventSuffix) // Eğer event adı sonundaki suffix silinsin ayarı açıksa
            {
                eventName = eventName.TrimEnd(eventBusConfig.EventNameSuffix.ToArray());
                eventName = eventName.TrimEnd(EventBusConfig.EventNameSuffix.ToArray()); // Suffix'i sil
            }
            return eventName;
            return eventName; // Düzenlenmiş event adını döner
        }


        // Subscriber adını oluşturur (appName.eventName şeklinde)
        public virtual string getSubName(string eventName)
        {
            return $"{EventBusConfig.SubscriberClientAppName}.{ProcessEventName(eventName)}"; // Ör: PaymentAPI.OrderCreated
            return $"{eventBusConfig.SubscriberClientAppName}.{ProcessEventName(eventName)}";
        }

        public virtual void Dispose()
        {
            eventBusConfig = null;
            EventBusConfig = null; // Config temizlenir
            subsManager.Clear(); // Tüm abonelikler hafızadan silinir
        }


        // Mesaj geldiğinde event'i çalıştıran ana fonksiyon
        public async Task<bool> ProcessEvent(string eventName, string message)
        {
            eventName = ProcessEventName(eventName);//event adını işler
            eventName = ProcessEventName(eventName); // Gelen event adını işler (prefix/suffix temizler)

            var processed = false;//işlenme durumu
            var processed = false; // Event işlendi mi durum bilgisi

            if (subsManager.HasSubscriptionsForEvent(eventName))//eğer event için abonelik varsa
            if (subsManager.HasSubscriptionsForEvent(eventName)) // Bu event için abone var mı?
            {
                var subscriptions = subsManager.GetHandlersForEvent(eventName);//abonelikleri alır

                using (var scope = serviceProvider.CreateScope()) // DI sisteminden yeni scope oluştur (Transient servisler için önemli)
                using (var scope = serviceProvider.CreateScope())//servis sağlayıcıdan yeni bir kapsam oluşturur
                {
                    foreach (var subscription in subscriptions) // Tüm handler'ları tek tek dolaş
                    foreach (var subscription in subscriptions)//her bir abonelik için
                    {
                        var handler = scope.ServiceProvider.GetService(subscription.HandlerType);//handler'ı alır
                        if (handler == null) continue;//eğer handler null ise devam et
                        var handler = scope.ServiceProvider.GetService(subscription.HandlerType); // Handler'ın instance'ını al

                        if (handler == null) continue; // Handler yoksa atla

                        // Event tipini alıyoruz (ör: OrderCreatedEvent)
                        var eventType = subsManager.GetEventTypeByName($"{EventBusConfig.EventNamePrefix}{eventName}{EventBusConfig.EventNameSuffix}");

                        var eventType = subsManager.GetEventTypeByName($"{eventBusConfig.EventNamePrefix}{eventName}{eventBusConfig.EventNameSuffix}");//event tipini alır
                        // Gelen JSON mesajı gerçek event tipine dönüştürüyoruz
                        var integrationEvent = Newtonsoft.Json.JsonConvert.DeserializeObject(message, eventType);

                        var integrationEvent = Newtonsoft.Json.JsonConvert.DeserializeObject(message, eventType);//mesajı event tipine deserialize eder

                        // Handler'ın Handle metodunu çağırıyoruz
                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);//somut handler tipini oluşturur
                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });//handler'ın Handle metodunu çağırır
                    }
                }

                processed = true; // İşlem başarılı oldu
                processed = true;//işlenme durumu true olur
            }

            return processed; // Event işlendi mi bilgisi döner
            return processed;//işlenme durumunu döner
        }

        public abstract void Publish(IntegrationEvent @event); // Event yayınla
        public abstract void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>; // Abone ol
        public abstract void UnSubscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>; // Abonelik iptal et

        public void publish(IntegrationEvent @event)
        {
            throw new NotImplementedException(); // Bu metod kullanılmıyor, override edilmemiş
        }

    }
}
