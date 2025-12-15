using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
namespace BasketService.Api.Extensions
{
    public static class AuthRegistration
    {
        // IServiceCollection için uzatma metodu: Authentication yapılandırması ekleniyor
        public static IServiceCollection ConfigureAuth(this IServiceCollection services, IConfiguration configuration)
        {
            // JWT için kullanılan gizli anahtarı appsettings içinden çekip SymmetricSecurityKey olarak oluşturuyor
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["AuthConfig:Secret"]));

            // Authentication servisini projeye ekleme
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;  // Varsayılan doğrulama şeması olarak JWT belirleniyor
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;     // Yetkisiz erişimde kullanılacak şema JWT olarak ayarlanıyor
            })
            .AddJwtBearer(options =>
            {
                // JWT token doğrulama ayarları
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,                     // Tokenı üreten kaynak doğrulanmıyor
                    ValidateAudience = false,                   // Token hedef kitlesi doğrulanmıyor
                    ValidateLifetime = true,                    // Token süresi kontrol ediliyor
                    ValidateIssuerSigningKey = true,            // Token imzası doğrulanacak
                    IssuerSigningKey = signingKey               // Tokenı doğrulamak için kullanılan güvenlik anahtarı
                };
            });

            return services;  // Servis koleksiyonunu geri döndürerek zinciri tamamlıyor
        }
    }
}