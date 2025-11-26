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
       
        public readonly IServiceProvider serviceProvider; // DI konteyner – servisleri oluşturmak için kullanılır
        public readonly IEventBusSubscriptionManager subsManager; // Event aboneliklerini tutan ve yöneten sınıf

       
        public EventBusConfig EventBusConfig { get; set; } // Event Bus ayarlarını tutan config sınıfı

        public BaseEventBus(EventBusConfig config, IServiceProvider serviceProvider)
        {
            EventBusConfig = config; // Config değerlerini property'e atıyoruz
            serviceProvider = serviceProvider; // DI servis sağlayıcısını class içine atıyoruz (BU SATIR HATALI çünkü kendine atıyorsun)
            subsManager = new InMemoryEventBusSubscriptionManager(ProcessEventName); // Abonelik yöneticisini oluşturuyoruz
           
        }



        // Event adını düzenleyen (prefix/suffix kırpan) metod
        public virtual string ProcessEventName(string eventName)
        {
          
            if (EventBusConfig.DeleteEventPrefix) // Eğer event adı başındaki prefix silinsin ayarı açıksa
            {
                
                eventName = eventName.TrimStart(EventBusConfig.EventNamePrefix.ToArray()); // Prefix'i sil
            }
          
            if (EventBusConfig.DeleteEventSuffix) // Eğer event adı sonundaki suffix silinsin ayarı açıksa
            {
               
                eventName = eventName.TrimEnd(EventBusConfig.EventNameSuffix.ToArray()); // Suffix'i sil
            }
          
            return eventName; // Düzenlenmiş event adını döner
        }


        // Subscriber adını oluşturur (appName.eventName şeklinde)
        public virtual string getSubName(string eventName)
        {
            return $"{EventBusConfig.SubscriberClientAppName}.{ProcessEventName(eventName)}"; // Ör: PaymentAPI.OrderCreated
          
        }

        public virtual void Dispose()
        {
          
            EventBusConfig = null; // Config temizlenir
            subsManager.Clear(); 
        }


        // Mesaj geldiğinde event'i çalıştıran ana fonksiyon
        public async Task<bool> ProcessEvent(string eventName, string message)
        {
           
            eventName = ProcessEventName(eventName); // Gelen event adını işler (prefix/suffix temizler)

            var processed = false;//işlenme durumu
           

           
            if (subsManager.HasSubscriptionsForEvent(eventName))//eğer event için abonelik varsa
          
            {
                var subscriptions = subsManager.GetHandlersForEvent(eventName);//abonelikleri alır
               

                using (var scope = serviceProvider.CreateScope()) // DI sisteminden yeni scope oluştur (Transient servisler için önemli        
                {
                    foreach (var subscription in subscriptions) // Tüm handler'ları tek tek dolaş
                
                    {
                        var handler = scope.ServiceProvider.GetService(subscription.HandlerType);//handler'ı alır
                        if (handler == null) continue;//eğer handler null ise devam et
                        

                        if (handler == null) continue; // Handler yoksa atla

                        // Event tipini alıyoruz (ör: OrderCreatedEvent)
                        

                        var eventType = subsManager.GetEventTypeByName($"{EventBusConfig.EventNamePrefix}{eventName}{EventBusConfig.EventNameSuffix}");//event tipini alır
                        // Gelen JSON mesajı gerçek event tipine dönüştürüyoruz
                       

                        var integrationEvent = Newtonsoft.Json.JsonConvert.DeserializeObject(message, eventType);//mesajı event tipine deserialize eder

                        // Handler'ın Handle metodunu çağırıyoruz
                       
                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);//somut handler tipini oluşturur
                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });//handler'ın Handle metodunu çağırır
                    }
                }

              
                processed = true;//işlenme durumu true olur
            }

            return processed; // Event işlendi mi bilgisi döner
           
        }

        public abstract void Publish(IntegrationEvent @event); // Event yayınla
        public abstract void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>; // Abone ol
        public abstract void UnSubscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>; // Abonelik iptal et
      
        public void publish(IntegrationEvent @event)
        {
            throw new NotImplementedException();
        }

    }
}
