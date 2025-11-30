using EventBus.Base.Events;

namespace PaymentService.Api.IntegrationEvents.Events
{
    public class OrderPaymentFailedIntegrationEvent : IntegrationEvent // Ödeme başarısız olduğunda diğer servisleri bilgilendirmek için oluşturulan event sınıfı.
                                                                       // IntegrationEvent'ten miras alıyor.
    {
        public Guid OrderId { get; } // Başarısız olan ödemenin ait olduğu siparişin benzersiz Id bilgisi.

        public string ErrorMessage { get; } // Ödemenin neden başarısız olduğuna dair hata mesajı.

        public OrderPaymentFailedIntegrationEvent(Guid orderId, string errorMessage) // Bu sınıftan nesne oluşturulurken çalışacak yapıcı metot.
        {
            OrderId = orderId; // Constructor'a gelen orderId parametresini OrderId property'sine atıyoruz.
            ErrorMessage = errorMessage; // Constructor'a gelen errorMessage parametresini ErrorMessage property'sine atıyoruz.
        }
    }

}
