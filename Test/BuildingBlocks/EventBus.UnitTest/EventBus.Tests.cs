using EventBus.Base.Abstraction;
using EventBus.Base.SubManager;
using EventBus.Factory;
using EventBus.UnitTest.Events.Events;
using EventBus.UnitTest.Events.EventsHandler;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework.Internal;
using RabbitMQ.Client;
using static EventBus.Base.SubManager.EventBusConfig;

namespace EventBus.UnitTest
{
    public class Tests
    {
        private ServiceCollection services;

        public Tests()
        {
            this.services = new ServiceCollection();//servis collection dan service provider oluþacak
            services.AddLogging(configure=>configure.AddConsole());

        }   

        [Test]
        public void subscirbe_event_on_rabbitmq_test()//q ,sub vb eventler için 
        {//ngleton olarak enjekte etmek, mikroservislerde bir sýnýfýn uygulama boyunca tek bir tane oluþturulup, tüm isteklerde ayný örneðin (instance) kullanýlacaðý anlamýna gelir.
           
            
            services.AddSingleton<IEventBus>(sp =>//her ýeventbus ýstendýðinde geriye donecek olan sey
            {

                return EventBusFactory.Create(GetRabbitMQConfig(),sp);//configi create methoduna yolladý 

            });


           var sp= services.BuildServiceProvider();//service provider oluþtu sp= service provider

            var eventBus = sp.GetRequiredService<IEventBus>();//IEVENTBUS ýstendýgýnde geriye donecek olan sey ÜSTTE TANIMLI OLAN 
            //artýk eventbus var 

            eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
            //bu olaya dair(OrderCreatedIntegrationEvent) bir iþlem olursa OrderCreatedIntegrationEventHandler altýnda tanýmlanan iþlemi yap demek

            eventBus.UnSubscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
            
        }

        [Test]
        public void subscirbe_event_on_azure_test()//q ,sub vb eventler için 
        {//ngleton olarak enjekte etmek, mikroservislerde bir sýnýfýn uygulama boyunca tek bir tane oluþturulup, tüm isteklerde ayný örneðin (instance) kullanýlacaðý anlamýna gelir.


            services.AddSingleton<IEventBus>(sp =>//her ýeventbus ýstendýðinde geriye donecek olan sey
            {
                EventBusConfig config = new EventBusConfig//bu ozrllikler EventBusConfig den gelecek olna methodlardýr 
                {
                    ConnectionRetryCount = 5,
                    SubscriberClientAppName = "EventBus.UnitTest",
                    DefaultTopicName = "SellingMicTopicBus",
                    EventBusTyp = EventBusType.AzureServiceBus,
                    EventNameSuffix = "Integrat" +
                    "ionEvent",//yazýlarýn trimlenmesi için
                    EventBusConnectionString = "Endpoint=sb://techbuddy.servicebus.windows.net/;SharedAccessKeyName=NewPolicyForYTVideos;SharedAccessKey=7sJghGWFOXaUaRblrbzOIIf4bQk6qkbTN/SEnKjXLpE="


                };
                // Noktalý virgül (;) ile ifadeyi sonlandýrýn. Baþka bir þey eklemeyin.

                return EventBusFactory.Create(config, sp);//configi create methoduna yolladý 

            });


            var sp = services.BuildServiceProvider();//service provider oluþtu sp= service provider

            var eventBus = sp.GetRequiredService<IEventBus>();//IEVENTBUS ýstendýgýnde geriye donecek olan sey ÜSTTE TANIMLI OLAN 
            //artýk eventbus var 

            eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
            //bu olaya dair(OrderCreatedIntegrationEvent) bir iþlem olursa OrderCreatedIntegrationEventHandler altýnda tanýmlanan iþlemi yap demek

            eventBus.UnSubscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
        }

        [Test]
        public void send_message_to_rabbitmq_test()
        {
            services.AddSingleton<IEventBus>(sp =>
            {
                return EventBusFactory.Create(GetRabbitMQConfig(), sp);
            });


            var sp = services.BuildServiceProvider();

            var eventBus = sp.GetRequiredService<IEventBus>();

            eventBus.publish(new OrderCreatedIntegrationEvent(1));
        }

        [Test]
        public void send_message_to_azure_test()
        {
            services.AddSingleton<IEventBus>(sp =>
            {
                return EventBusFactory.Create(GetAzureConfig(), sp);
            });


            var sp = services.BuildServiceProvider();

            var eventBus = sp.GetRequiredService<IEventBus>();

            eventBus.publish(new OrderCreatedIntegrationEvent(1));
        }
        private EventBusConfig GetAzureConfig()
        {
            return new EventBusConfig()
            {
                ConnectionRetryCount = 5,
                SubscriberClientAppName = "EventBus.UnitTest",
                DefaultTopicName = "SellingMicTopicBus",
                EventBusTyp = EventBusType.AzureServiceBus,
                EventNameSuffix = "IntegrationEvent",//yazýlarýn trimlenmesi için
                EventBusConnectionString = "Endpoint=sb://techbuddy.servicebus.windows.net/;SharedAccessKeyName=NewPolicyForYTVideos;SharedAccessKey=7sJghGWFOXaUaRblrbzOIIf4bQk6qkbTN/SEnKjXLpE="
            };
        }

        private EventBusConfig GetRabbitMQConfig()
        {
            return new EventBusConfig()
            {
                ConnectionRetryCount = 5,
                SubscriberClientAppName = "EventBus.UnitTest",
                DefaultTopicName = "SellingBuddyTopicName",
                EventBusTyp = EventBusType.RabbitMQ,
                EventNameSuffix = "IntegrationEvent",
                Connection = new ConnectionFactory()
                {
                    HostName = "localhost",
                    Port = 15672,
                    UserName = "guest",
                    Password = "guest"
                }
            };
        }
    }
    
}