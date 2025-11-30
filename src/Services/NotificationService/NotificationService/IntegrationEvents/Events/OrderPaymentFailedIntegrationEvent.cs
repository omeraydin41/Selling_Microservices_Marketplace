using EventBus.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.IntegrationEvents.Events
{
    public class OrderPaymentFailedIntegrationEvent : IntegrationEvent
    {
        // Bu, diğer sistemlere 'ödeme başarısız oldu' bilgisini ileten özel bir mesaj sınıfıdır.

        public Guid OrderId { get; }
        // 'public Guid OrderId' demek, bu mesajın taşıdığı önemli verilerden birinin
        // siparişin benzersiz kimliği (ID'si) olduğunu belirtir.
        // 'Guid' (Global Unique Identifier) benzersiz bir numara formatıdır.
        // '{ get; }' ise bu değerin dışarıdan sadece okunabileceği (yani mesaj oluştuktan sonra değiştirilemeyeceği) anlamına gelir.

        public string ErrorMessage { get; }
        // 'public string ErrorMessage' demek, ödemenin neden başarısız olduğuna dair
        // ayrıntılı bir metin mesajı (string) taşıyacağını belirtir.
        // '{ get; }' aynı şekilde bu hata mesajının da sadece okunabilir olduğunu belirtir.

        public OrderPaymentFailedIntegrationEvent(Guid orderId, string errorMessage)
        // Bu, sınıfın 'yapıcı metodu' (constructor) denilen özel bir metodudur.
        // Bu sınıfın bir nesnesini oluşturmak istediğimizde, bu metodu kullanırız ve
        // parantez içindeki değerleri (orderId ve errorMessage) ona vermek zorundayız.
        {
            OrderId = orderId;
            // Yapıcı metoda dışarıdan gelen 'orderId' değerini, sınıfın kendi 'OrderId' özelliğine atarız.

            ErrorMessage = errorMessage;
            // Yapıcı metoda dışarıdan gelen 'errorMessage' değerini, sınıfın kendi 'ErrorMessage' özelliğine atarız.
        }
    }
}
