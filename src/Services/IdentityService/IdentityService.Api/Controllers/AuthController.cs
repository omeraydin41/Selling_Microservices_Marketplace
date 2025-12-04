using IdentityService.Api.Application.Models;
using IdentityService.Api.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Api.Controllers
{
    [Route("api/[controller]")]
    // Bu controller'ın API adresini belirtir.
    // [controller] yerine sınıfın adı (Auth) otomatik yazılır → "api/Auth"

    [ApiController]
    // Bu sınıfın bir API controller olduğunu söyler.
    // Model doğrulama, binding gibi özellikler otomatik çalışır.

    public class AuthController : ControllerBase
    {
        private readonly IIdentityService identityService;
        // Kimlik doğrulama işlemlerini yapan servisi tutan değişken.

        public AuthController(IIdentityService identityService)
        {
            // identityService, dışarıdan (Dependency Injection) verilir.
            this.identityService = identityService;
        }

        [HttpPost]
        // Bu metodun HTTP POST isteğiyle çalışacağını belirtir.
        public async Task<IActionResult> Login([FromBody] LoginRequestModel loginRequestModel)
        {
            // API'ye gelen JSON verisi LoginRequestModel'e dönüştürülür.
            // [FromBody] → veriyi body’den al demek.

            var result = await identityService.Login(loginRequestModel);
            // Kullanıcının giriş bilgileri servise gönderilir.
            // Servis token oluşturur ve geri döner.

            return Ok(result);
            // 200 OK ile birlikte token ve kullanıcı bilgileri API’ye döndürülür.
        }
    }

}
