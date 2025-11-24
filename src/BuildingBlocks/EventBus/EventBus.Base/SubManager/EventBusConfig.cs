using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base.SubManager
{
    public class EventBusConfig
    {
        public int ConnectionRetryCount { get; set; } = 5;//bağlanırken rabbitmq sunucusuna 5 kere deneyecek
        //dev ortamınde ağ hatası olursa diye tek sefer bağlanmak yerine 5 kere deneyecek

        public string DefaultTopicName { get; set; } = "SellingEventBus";//varsayılan konu adı
        //kullanılacak toppic namedir bunun latında kuyruklarımız oluşturacağız sıstem hata almasın diye deafault topic name verdık 

        public string EventBusConnectionString { get; set; } = String.Empty;//bağlantı dizesi
        //rabbitmq sunucusuna bağlanmak için gerekli olan bağlantı dizesidir

        public string SubscriberClientAppName { get; set; } = String.Empty;//abone istemci uygulama adı
        //hangı servis yenı bir q yaratacak sa o servisin adı yazacak örnek order servis : q kuyruk demek 
        //mesajların (bizim bağlamımızda Event'lerin) geçici olarak depolandığı bir veri yapısıdır.

        public string EventNamePrefix { get; set; } = String.Empty;
        // Olay (Event) adının başına eklenecek dizedir (Örn: "Dev.").

        public string EventNameSuffix { get; set; } = "IntegrationEvent";
        // Olay (Event) adının sonuna eklenecek dizedir (Örn: "IntegrationEvent").

        public EventBusType EventBusTyp { get; set; } = EventBusType.RabbitMQ;
        //default olarak rabbitmq kullanacak

        public object Connection { get; set; }
        //bu object olma sebebi herhangı bir servis bu baseyı kullaırken bu baseyı referans almış projeler 
        //mutlaka rabit mq client yuklu olması gerekırdı çünku buraya yazılan clıent rabbırmq ya ait 
        //bu yuxden object kı rabbıt mq veya azure kullanıcıları buraya kendi clientlarını atayabilirler

        public bool DeleteEventPrefix => !String.IsNullOrEmpty(EventNamePrefix);

        public bool DeleteEventSuffix => !String.IsNullOrEmpty(EventNameSuffix);

        public enum EventBusType
        {
            RabbitMQ = 0,
            AzureServiceBus = 1
        }
    }
}
