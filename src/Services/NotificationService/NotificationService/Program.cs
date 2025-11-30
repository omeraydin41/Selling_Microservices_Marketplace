
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Base.SubManager;
using EventBus.Factory;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.IntegrationEvents.EventHandlers;
using NotificationService.IntegrationEvents.Events;
using RabbitMQ.Client;
using Serilog;
using static EventBus.Base.SubManager.EventBusConfig;

class Program
{


    private static void Main(string[] args)//dınlenecek eventler paymentsucces ve fail eventleri
    {
        ServiceCollection services = new ServiceCollection();

        ConfigureServices(services);
        var sp=services.BuildServiceProvider();
        IEventBus eventBus= sp.GetRequiredService<IEventBus>();//required gereklı demek : bu eventbus aracılığı ıle sun işlemi yapacağız hangı q lar dınlenılmek ıstenırse 


        eventBus.Subscribe<OrderPaymentSuccessIntegrationEvent, OrderPaymentSuccessIntegrationEventHandler>();//bu eventlere abone oldu //odeme işlemi başarılı  olursa eventı
        eventBus.Subscribe<OrderPaymentFailedIntegrationEvent, OrderPaymentFailedIntegrationEventHandler>(); ;//bu eventlere abone oldu //odeme işlemi başarısız olursa eventı

        Log.Logger.Information("Application is Running....");
    }
    static void ConfigureServices(ServiceCollection services)
    {
        services.AddTransient<OrderPaymentFailedIntegrationEventHandler>();
        services.AddTransient<OrderPaymentSuccessIntegrationEventHandler>();

        services.AddSingleton(sp =>
        {
            EventBusConfig config = new()//bizden eventbus ıstenırse burası çalışacak 
            {
                ConnectionRetryCount = 5,
                EventNameSuffix = "IntegrationEvent",//isim kısaltma 
                SubscriberClientAppName = "NotificationService",
                EventBusTyp = EventBusType.RabbitMQ,
                Connection = new ConnectionFactory()
                {
                    HostName = "c_rabbitmq"
                }
            };

            return EventBusFactory.Create(config, sp);
        });
    }
    //private static IConfiguration serilogConfiguration
    //{
    //    get
    //    {
    //        return new ConfigurationBuilder()
    //            .SetBasePath(System.IO.Directory.GetCurrentDirectory())
    //            .AddJsonFile($"Configurations/serilog.json", optional: false)
    //            .AddJsonFile($"Configurations/serilog.{env}.json", optional: true)
    //            .AddEnvironmentVariables()
    //            .Build();
    //    }
    //}
}
