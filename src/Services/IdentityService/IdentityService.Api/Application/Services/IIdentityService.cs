using IdentityService.Api.Application.Models;

namespace IdentityService.Api.Application.Services
{
    public interface IIdentityService
    {
        // Login isteğini işleyip giriş sonucunu (LoginResponseModel) döndüren asenkron görev metodu
        Task<LoginResponseModel> Login(LoginRequestModel requestModel);//ıstek ve cebau ı işeyıp dondur

    }
}
