namespace IdentityService.Api.Application.Models
{
    public class LoginResponseModel
    {
        public string UserName { get; set; }//cevap olarak kullanıcı adı döner

        public string UserToken { get; set; }//cevap olarak kullanıcı tokenı döner passworde karşilık 
    }
}
