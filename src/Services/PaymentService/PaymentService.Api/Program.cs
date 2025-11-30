using EventBus.Base.Abstraction;
using EventBus.Base.SubManager;
using EventBus.Factory;
using PaymentService.Api.IntegrationEvents.EventHandler;
using static EventBus.Base.SubManager.EventBusConfig;

var builder = WebApplication.CreateBuilder(args); // Uygulamayý baþlatan builder nesnesi oluþturulur. Services ve config iþlemleri burada yapýlýr.

// ***************
//  ConfigureServices EÞDEÐERÝ (Startup.cs ? Program.cs)
// ***************

builder.Services.AddControllers(); // Controller sýnýflarýný projeye ekler ve API'nin çalýþmasýný saðlar.

builder.Services.AddEndpointsApiExplorer(); // Minimal API ve Swagger'ýn endpointleri taramasý için gerekli alt yapý.

builder.Services.AddSwaggerGen(c => // Swagger dokümantasyon ayarlarýný yapar.
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo // Swagger sayfasý için bilgi oluþturulur.
    {
        Title = "PaymentService.Api", // Swagger baþlýðý
        Version = "v1" // API versiyon bilgisi
    });
});

builder.Services.AddLogging(configure => // Loglama için ayarlar yapýlýr.
{
    configure.AddConsole(); // Loglarýn konsola yazýlmasýný saðlar.
    configure.AddDebug();   // Loglarýn Visual Studio debug penceresine yazýlmasýný saðlar.
});

builder.Services.AddTransient<OrderStartedIntegrationEventHandler>();
// OrderStartedIntegrationEvent için event handler eklenir. Her çaðrýda yeni instance oluþturulur.

builder.Services.AddSingleton<IEventBus>(sp => // EventBus’ý Singleton olarak ekler. Tüm uygulamada 1 adet olur.
{
    EventBusConfig config = new() // EventBus yapýlandýrmasý oluþturulur.
    {
        ConnectionRetryCount = 5, // RabbitMQ baðlantýsý baþarýsýz olursa tekrar deneme sayýsý.
        EventNameSuffix = "IntegrationEvent", // Event isimlerinin sonuna eklenecek suffix.
        SubscriberClientAppName = "PaymentService", // Bu servisin RabbitMQ içinde görünen adý.
        EventBusTyp = EventBusType.RabbitMQ, // Kullanýlacak event bus türü (RabbitMQ).
        Connection = new RabbitMQ.Client.ConnectionFactory() // RabbitMQ baðlantý bilgileri.
        {
            HostName = "localhost", // RabbitMQ sunucu adresi
            Port = 5672, // RabbitMQ baðlantý portu (doðru olan port budur)
            UserName = "guest", // Varsayýlan kullanýcý adý
            Password = "guest"  // Varsayýlan þifre
        }
    };

    return EventBusFactory.Create(config, sp); // EventBus’ý oluþturur ve döndürür.
});

var app = builder.Build(); // Uygulama nesnesi oluþturulur ve middleware'ler için hazýr hale gelir.

// ***************
//  Configure EÞDEÐERÝ (Startup.cs ? Program.cs)
// ***************

if (app.Environment.IsDevelopment()) // Uygulama geliþtirme ortamýnda çalýþýyorsa (production deðilse)
{
    app.UseDeveloperExceptionPage(); // Hata sayfasýný etkinleþtirir (yalnýzca development’ta).
    app.UseSwagger(); // Swagger endpoint’ini aktif eder.
    app.UseSwaggerUI(c => // Swagger UI arayüzünü etkinleþtirir.
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PaymentService.Api v1"); // Swagger JSON dosyasýnýn yolunu belirtir.
    });
}

app.UseHttpsRedirection(); // HTTP isteklerini otomatik olarak HTTPS'e yönlendirir.

app.UseAuthorization(); // Authorization middleware’ini aktif eder.

app.MapControllers(); // Controller endpoint'lerini uygulamaya map eder (API çalýþýr hale gelir).

app.Run(); // Uygulamayý çalýþtýrýr ve sonsuza kadar dinlemeye alýnýr.
