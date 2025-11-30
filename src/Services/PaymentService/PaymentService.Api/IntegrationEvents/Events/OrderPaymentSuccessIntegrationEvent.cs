using EventBus.Base.Events;

namespace PaymentService.Api.IntegrationEvents.Events
{
    public class OrderPaymentSuccessIntegrationEvent : IntegrationEvent // OrderPaymentSuccessIntegrationEvent adında bir sınıf oluşturuyoruz.
                                                                        // IntegrationEvent'ten miras alıyor.
    {
        public Guid OrderId { get; } // Bu etkinliğe ait siparişin Id bilgisini tutuyor. Sadece okunabilir (get var set yok).

        public OrderPaymentSuccessIntegrationEvent(Guid orderId) => OrderId = orderId;
        // Yapıcı metot (constructor). Bu sınıftan bir nesne oluşturulduğunda orderId parametresi alınır
        // ve sınıftaki OrderId property'sine atanır.
    }

}
