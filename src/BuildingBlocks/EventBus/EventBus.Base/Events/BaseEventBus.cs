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

        private EventBusConfig eventBusConfig;//event bus konfigürasyonu
        public BaseEventBus(EventBusConfig config, IServiceProvider serviceProvider)
        {
            eventBusConfig = config;//event bus konfigürasyonunu atar
            serviceProvider = serviceProvider;//servis sağlayıcıyı atar
            subsManager = new InMemoryEventBusSubscriptionManager(ProcessEventName);//abonelik yöneticisini oluşturur
        }

       

        public virtual string ProcessEventName(string eventName)
        {
            if (eventBusConfig.DeleteEventPrefix)//eğer event adı ön eki silinecekse
            {
                eventName = eventName.TrimStart(eventBusConfig.EventNamePrefix.ToArray());
            }
            if (eventBusConfig.DeleteEventSuffix)//eğer event adı son eki silinecekse
            {
                eventName = eventName.TrimEnd(eventBusConfig.EventNameSuffix.ToArray());
            }
            return eventName;
        }


        public virtual string getSubName(string eventName)
        {
            return $"{eventBusConfig.SubscriberClientAppName}.{ProcessEventName(eventName)}";
        }
        public virtual void Dispose()
        {
            eventBusConfig = null;
        }

        public async Task<bool> ProcessEvent(string eventName, string message)
        {
            eventName = ProcessEventName(eventName);//event adını işler

            var processed = false;//işlenme durumu

            if (subsManager.HasSubscriptionsForEvent(eventName))//eğer event için abonelik varsa
            {
                var subscriptions = subsManager.GetHandlersForEvent(eventName);//abonelikleri alır

                using (var scope = serviceProvider.CreateScope())//servis sağlayıcıdan yeni bir kapsam oluşturur
                {
                    foreach (var subscription in subscriptions)//her bir abonelik için
                    {
                        var handler = scope.ServiceProvider.GetService(subscription.HandlerType);//handler'ı alır
                        if (handler == null) continue;//eğer handler null ise devam et

                        var eventType = subsManager.GetEventTypeByName($"{eventBusConfig.EventNamePrefix}{eventName}{eventBusConfig.EventNameSuffix}");//event tipini alır

                        var integrationEvent = Newtonsoft.Json.JsonConvert.DeserializeObject(message, eventType);//mesajı event tipine deserialize eder

                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);//somut handler tipini oluşturur
                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });//handler'ın Handle metodunu çağırır
                    }
                }
                processed = true;//işlenme durumu true olur
            }
            return processed;//işlenme durumunu döner
        }

        public abstract void Publish(IntegrationEvent @event);
        public abstract void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
        public abstract void UnSubscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;

        public void publish(IntegrationEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}
