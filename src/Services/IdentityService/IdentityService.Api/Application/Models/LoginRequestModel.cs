namespace IdentityService.Api.Application.Models
{
    public class LoginRequestModel//giriş ıstek modelı
    {
        public string UserName { get; set; }//girerken kullanıcı adı alınır 

        public string Password { get; set; }//girerken parola alınır
    }
}
