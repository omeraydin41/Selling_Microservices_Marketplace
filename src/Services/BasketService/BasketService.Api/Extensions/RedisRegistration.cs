using StackExchange.Redis;

namespace BasketService.Api.Extensions
{
    // Redis ile ilgili servis kayıtlarını tutmak için kullanılan static sınıf
    public static class RedisRegistration
    {
        // IServiceProvider'a extension method olarak eklenen Redis konfigürasyon metodu
        // Bu metot Redis bağlantısını oluşturur ve ConnectionMultiplexer nesnesini döner
        public static ConnectionMultiplexer ConfigureRedis(
            this IServiceProvider services,   // Dependency Injection konteynerini temsil eder
            IConfiguration configuration      // Uygulama ayarlarını (appsettings.json vb.) okumak için kullanılır
        )
        {
            // appsettings.json içindeki "RedisSettings:ConnectionString" değerini alır
            // true parametresi: bağlantı dizesindeki bilinmeyen ayarları yoksaymasını sağlar
            var redisConf = ConfigurationOptions.Parse(
                configuration["RedisSettings:ConnectionString"], // Redis bağlantı cümlesi
                true                                              // AbortOnConnectFail = false benzeri tolerans sağlar
            );

            // Redis sunucu adresi DNS üzerinden çözümlenecekse bunu aktif eder
            // Özellikle Docker, Kubernetes gibi ortamlarda önemlidir
            redisConf.ResolveDns = true;

            // Belirlenen ayarlar ile Redis sunucusuna bağlantı kurar
            // ConnectionMultiplexer, Redis ile haberleşmeyi yöneten ana nesnedir
            return ConnectionMultiplexer.Connect(redisConf);
        }
    }

}
