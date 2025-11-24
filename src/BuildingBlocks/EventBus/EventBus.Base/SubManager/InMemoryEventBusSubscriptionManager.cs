using EventBus.Base.Abstraction;
using EventBus.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base.SubManager
{
    public class InMemoryEventBusSubscriptionManager : IEventBusSubscriptionManager
    {
        private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;
        private readonly List<Type> _eventTypes;
        public event EventHandler<string> OnEventRemoved;
        public Func<string, string> eventKeyGetter;
        //public bool IsEmpty => throw new NotImplementedException();

        public InMemoryEventBusSubscriptionManager(Func<string,string>eventNameGetter)//kuyruk yönetici sınıfının kurucusu
        {//strıng alıp string döndüren bir fonksiyon 
            _handlers = new Dictionary<string, List<SubscriptionInfo>>();//event isimleri ile bu eventlere ait handler tiplerini tutan sözlük
            _eventTypes = new List<Type>();//kayıtlı event tiplerini tutan liste
            eventKeyGetter = eventNameGetter;//event isimlerini almak için kullanılan fonksiyon

        }

        public bool IsEmpty => !_handlers.Keys.Any();//abonelik yöneticisinin boş olup olmadığını kontrol eder
        public void Clear() =>_handlers.Clear();//tüm abonelikleri temizler


        public void AddSubscription<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
        {
            var eventName = GetEventKey<T>();//eventin anahtarını alır

            AddSubscription(typeof(TH), eventName);//aboneliği ekler
            if (!_eventTypes.Contains(typeof(T)))//eğer event tipi listede yoksa
            {
                _eventTypes.Add(typeof(T));//event tipini listeye ekler
            }


        }

        //public void Clear()
        //{
        //    throw new NotImplementedException();
        //}
        private void AddSubscription(Type handlerType, string eventName)
        {
            if (!HasSubscriptionsForEvent(eventName))//eğer event için abonelik yoksa
            {
                _handlers.Add(eventName, new List<SubscriptionInfo>());//yeni bir abonelik listesi oluşturur
            }
            if (_handlers[eventName].Any(s => s.HandlerType == handlerType))//eğer aynı handler tipi zaten varsa
            {
                throw new ArgumentException(
                    $"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));//hata fırlatır
            }
            _handlers[eventName].Add(SubscriptionInfo.Typed(handlerType));//abonelik bilgisi ekler
        }


        public void RemoveSubscription<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
        {
            var handlerToRemove = FindSubscriptionToRemove<T, TH>();//kaldırılacak aboneliği bulur
            var eventName = GetEventKey<T>();//eventin anahtarını alır
            RemoveHandler(eventName, handlerToRemove);//aboneliği kaldırır
        }


        private void RemoveHandler(string eventName, SubscriptionInfo subsToRemove)
        {
            if (subsToRemove != null)//eğer kaldırılacak abonelik varsa
            {
                _handlers[eventName].Remove(subsToRemove);//aboneliği kaldırır
                if (!_handlers[eventName].Any())//eğer o event için başka abonelik kalmadıysa
                {
                    _handlers.Remove(eventName);//eventi sözlükten kaldırır
                    var eventType = _eventTypes.SingleOrDefault(e => e.Name == eventName);//event tipini bulur
                    if (eventType != null)//eğer event tipi bulunduysa
                    {
                        _eventTypes.Remove(eventType);//event tipini listeden kaldırır
                    }
                    OnEventRemoved?.Invoke(this, eventName);//event kaldırıldığında olayı tetikler
                }
            }
        }
        public IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent
        {
            var key = GetEventKey<T>();
            return GetHandlersForEvent(key);
        }
       // public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName) => _handlers(eventName);

        private void RaiseOnEventRemoved(string eventName)
        {
            var handler = OnEventRemoved;
            handler?.Invoke(this, eventName);
        }
        private SubscriptionInfo FindSubscriptionToRemove<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = GetEventKey<T>();//eventin anahtarını alır
            return FindSubscriptionToRemove(eventName, typeof(TH));//aboneliği bulur
        }
        private SubscriptionInfo FindSubscriptionToRemove(string eventName, Type handlerType)
        {
            if (!HasSubscriptionsForEvent(eventName))//eğer event için abonelik yoksa
            {
                return null;//null döner
            }
            return _handlers[eventName].SingleOrDefault(s => s.HandlerType == handlerType);//aboneliği bulur ve döner
        }
        public bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent
        {
            var key = GetEventKey<T>();//eventin anahtarını alır
            return HasSubscriptionsForEvent(key);//abonelik olup olmadığını kontrol eder
        }
        //public bool HasSubscriptionsForEvent(string eventName)=> _handlers.ContainsKey(eventName);//event için abonelik olup olmadığını kontrol eder
        public string GetEventKey<T>()
        {
           string eventName = typeof(T).Name;//eventin adını alır
            return eventKeyGetter(eventName);//eventin anahtarını döner
        }

        public Type GetEventTypeByName(string eventName)
        {
            throw new NotImplementedException();
        }


        public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName)
        {
            throw new NotImplementedException();
        }

        

        public bool HasSubscriptionsForEvent(string eventName)
        {
            throw new NotImplementedException();
        }

      
    }
}
