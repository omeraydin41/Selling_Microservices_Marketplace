using EventBus.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base.Abstraction
{
    public interface IEventBusSubscriptionManager
    {
        bool IsEmpty { get; }//abonelik yöneticisinin boş olup olmadığını kontrol eder

        event EventHandler<string> OnEventRemoved;//bir event kaldırıldığında tetiklenen olay

        void AddSubscription<T, TH>()//bize IntegrationEvent ve IIntegrationEventHandler verecek ve bız de bunu rabbit e bildireceğiz
            where T : IntegrationEvent//T tipi IntegrationEvent türünden türemiş olmalı
            where TH : IIntegrationEventHandler<T>;//TH tipi IIntegrationEventHandler<T> türünden türemiş olmalı

        void RemoveSubscription<T, TH>()//bir bileşenin artık belirli bir olayı dinlemeyi bıraktığını Event Bus'a bildirdiği işlemdir.
            where TH : IIntegrationEventHandler<T>//TH tipi IIntegrationEventHandler<T> türünden türemiş olmalı
            where T : IntegrationEvent;//tipi IntegrationEvent türünden türemiş olmalı

        bool HasSubscriptionsForEvent<T>()//belirli bir türdeki olaya abonelik olup olmadığını kontrol eder
            where T : IntegrationEvent;//T tipi IntegrationEvent türünden türemiş olmalı

        bool HasSubscriptionsForEvent(string eventName);//belirli bir ada sahip olaya abonelik olup olmadığını kontrol eder

        Type GetEventTypeByName(string eventName);//belirli bir ada sahip olaya karşılık gelen türü alır

        void Clear();//tüm abonelikleri temizler

        IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>()//belirli bir türdeki olaya karşılık gelen tüm abonelik bilgilerini alır
            where T : IntegrationEvent;//T tipi IntegrationEvent türünden türemiş olmalı

        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);
        //belirli bir ada sahip olaya karşılık gelen tüm abonelik bilgilerini alır

        string GetEventKey<T>();//belirli bir türdeki olayın anahtarını alır
           
    }
}
