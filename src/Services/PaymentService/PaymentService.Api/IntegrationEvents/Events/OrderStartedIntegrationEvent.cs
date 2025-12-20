using EventBus.Base.Events;
namespace PaymentService.Api.IntegrationEvents.Events
{
    public class OrderStartedIntegrationEvent : IntegrationEvent
    {
        public Guid OrderId { get; set; }//hangı orderin basladıgını bilmek ıcın

        public OrderStartedIntegrationEvent(string userName)
        {

        }

        public OrderStartedIntegrationEvent(string userName, Guid orderId)//order ıd yı dışardan da alabılırız 
        {
            OrderId = orderId;
        }
    }
}
