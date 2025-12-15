using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
namespace BasketService.Api.Extensions
{
    public static class ConsulRegistration
    {
        // Consul client konfigurasyonunu servis koleksiyonuna ekleyen extension metodu
        public static IServiceCollection ConfigureConsul(this IServiceCollection services, IConfiguration configuration)
        {
            // ConsulClient'i singleton olarak DI container'a ekliyor
            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
            {
                var address = configuration["ConsulConfig:Address"]; // Consul adresini appsettings'ten okuyor
                consulConfig.Address = new Uri(address);             // Consul adresini URI formatına çevirip ayarlıyor
            }));

            return services; // Servis koleksiyonunu geri döndürüyor
        }

        // Uygulamanın Consul'e kendini kaydetmesini sağlayan extension metodu
        public static IApplicationBuilder RegisterWithConsul(this IApplicationBuilder app, IHostApplicationLifetime lifetime, IConfiguration configuration)
        {
            var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();              // Consul client'i DI'dan alıyor

            var loggingFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();            // Logger factory alınıyor
            var logger = loggingFactory.CreateLogger<IApplicationBuilder>();                              // Logger oluşturuluyor

            var uri = configuration.GetValue<Uri>("ConsulConfig:ServiceAddress");                         // Servisin kendi adresi
            var serviceName = configuration.GetValue<string>("ConsulConfig:ServiceName");                 // Servisin adı
            var serviceId = configuration.GetValue<string>("ConsulConfig:ServiceId");                     // Servisin id'si

            // Consul'e kaydedilecek olan servis bilgileri oluşturuluyor
            var registration = new AgentServiceRegistration()
            {
                ID = serviceId ?? "BasketService",                   // Servisin benzersiz id'si
                Name = serviceName ?? "BasketService",               // Servisin adı
                Address = $"{uri.Host}",                             // Servis host bilgisi
                Port = uri.Port,                                     // Servis port bilgisi
                Tags = new[] { serviceName, serviceId }              // Servisi etiketlemek için tag listesi
            };

            logger.LogInformation("Registering with Consul");        // Log: Consul'e kayıt başlıyor
            consulClient.Agent.ServiceDeregister(registration.ID).Wait(); // Aynı ID ile eski kayıt varsa siliyor
            consulClient.Agent.ServiceRegister(registration).Wait();      // Servisi Consul'e kaydediyor

            // Uygulama kapanırken Consul kaydının silinmesini sağlıyor
            lifetime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation("Deregistering from Consul");       // Log: Consul'den çıkış yapılıyor
                consulClient.Agent.ServiceDeregister(registration.ID).Wait(); // Servis deregister işlemi
            });

            return app; // IApplicationBuilder geri döndürülüyor
        }
    }
}
