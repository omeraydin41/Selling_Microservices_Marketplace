using IdentityService.Api.Application.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityService.Api.Application.Services
{
    public class IdentityService : IIdentityService
    {
        public Task<LoginResponseModel> Login(LoginRequestModel requestModel)
        {
            // Burada normalde veritabanı işlemleri yapılır.
            // Kullanıcının adı/şifresi doğru mu kontrol edilir.
            // Şu anlık DB işlemi yok, direkt token üretiyoruz.

            var claims = new Claim[]
            {
            // Kullanıcının benzersiz ID bilgisi (burada username kullanılmış)
            new Claim(ClaimTypes.NameIdentifier, requestModel.UserName),

            // Kullanıcının görünen adı (sabit yazılmış)
            new Claim(ClaimTypes.Name, "ömer aydın"),
            };

            // Token'ı imzalamak için gizli anahtar oluşturuluyor.
            // Bu değer uzun ve karmaşık olmalıdır, aksi halde güvenlik zayıf olur.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("OmerAydinSecretKeyShouldBeLong"));

            // Token'ın hangi algoritma ile imzalanacağını belirtiyoruz (HmacSha256)
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Token'ın geçerlilik süresi (şu an 10 gün)
            var expiry = DateTime.Now.AddDays(10);

            // JWT token nesnesi oluşturuluyor
            var token = new JwtSecurityToken(
                claims: claims,            // Token içine hangi bilgiler yazılacak claim içerisndeki bilgiler
                expires: expiry,           // Ne zamana kadar geçerli 10 gun
                signingCredentials: creds, // Hangi anahtar ve algoritma ile imzalandı algorştma turunu de verdık 
                notBefore: DateTime.Now    // Bu zamandan önce geçerli olmasın
            );

            // Token'ı string formatına çeviriyoruz token oluşturuldu 
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(token);

            // Kullanıcıya döneceğimiz model
            LoginResponseModel response = new()
            {
                UserToken = encodedJwt,           // Oluşturulan token
                UserName = requestModel.UserName  // Giriş yapan kullanıcının adı
            };

            // Asenkron bir sonuç döndürüyoruz
            return Task.FromResult(response);
        }
    }
}
