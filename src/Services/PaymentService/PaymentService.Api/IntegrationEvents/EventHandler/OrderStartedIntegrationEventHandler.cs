using EventBus.Base.Abstraction;
using EventBus.Base.Events;
using PaymentService.Api.IntegrationEvents.Events;

namespace PaymentService.Api.IntegrationEvents.EventHandler
{
    public class OrderStartedIntegrationEventHandler : IIntegrationEventHandler<OrderStartedIntegrationEvent>
    // OrderStartedIntegrationEvent isimli eventi yakalayan (handle eden) event handler sınıfı.
    // IIntegrationEventHandler<OrderStartedIntegrationEvent> arayüzünü implement ediyor.
    {
        private readonly IConfiguration configuration; // Uygulama ayarlarına erişmek için kullanılan yapı.
        private readonly IEventBus eventBus; // Event publish etmek için event bus nesnesi.
        private readonly ILogger<OrderStartedIntegrationEventHandler> logger; // Loglama yapmak için logger.

        public OrderStartedIntegrationEventHandler(IConfiguration configuration, IEventBus eventBus, ILogger<OrderStartedIntegrationEventHandler> logger)
        // Constructor: bu sınıf oluşturulurken dışarıdan configuration, eventBus ve logger alıyoruz.
        {
            this.configuration = configuration; // Alınan configuration nesnesini sınıftaki değere atıyoruz.
            this.eventBus = eventBus; // Alınan eventBus nesnesini sınıftaki değere atıyoruz.
            this.logger = logger; // Alınan logger nesnesini sınıftaki değere atıyoruz.
        }

        public Task Handle(OrderStartedIntegrationEvent @event)
        // Event tetiklendiğinde çalışan metot. OrderStartedIntegrationEvent türünde bir event alır.
        {
            // Fake payment process
            string keyword = "PaymentSuccess"; // App settings içinden bu anahtar ile değer okunacak.
            bool paymentSuccessFlag = configuration.GetValue<bool>(keyword);
            // Ayarlardan PaymentSuccess anahtarının değerini okur. true ise ödeme başarılı olacak.

            IntegrationEvent paymentEvent = paymentSuccessFlag
                ? new OrderPaymentSuccessIntegrationEvent(@event.OrderId) // Ödeme başarılıysa success event oluşturulur.
                : new OrderPaymentFailedIntegrationEvent(@event.OrderId, "This is a fake error message");
            // Ödeme başarısızsa failed event oluşturulur ve hata mesajı eklenir.

            logger.LogInformation(
                $"OrderStartedIntegrationEventHandler in PaymentService is fired with PaymentSuccess: {paymentSuccessFlag}, orderId: {@event.OrderId}");
            // Hangi eventin işlendiğini, ödeme durumunu ve OrderId'yi loglar.

            eventBus.publish(paymentEvent);
            // Oluşturulan success veya failed eventini event bus üzerinden publish eder.

            return Task.CompletedTask;
            // Metot async olduğu için tamamlandı bilgisini döner.
        }
    }

}
