using EventBus.Base.Abstraction;
using NotificationService.IntegrationEvents.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.IntegrationEvents.EventHandlers
{
    class OrderPaymentSuccessIntegrationEventHandler : IIntegrationEventHandler<OrderPaymentSuccessIntegrationEvent>
    {//odeme başarılı eventıDİR =  BU CLASS IIntegrationEventHandler DEN TUREMELI VE OrderPaymentSuccessIntegrationEvent CLASSINI HANDLE ETMELİ 

        public Task Handle(OrderPaymentSuccessIntegrationEvent @event)//hangı eventın işleneceğide burda belirtılmeli ve bır nesne alındı 
        {
            // Send Fail Notification (Sms, EMail, Push)

            Log.Logger.Information($"Order Payment Success with OrderId: {@event.OrderId}");
            // Bu satır, sistemde bir olayın başarılı bir şekilde gerçekleştiğini kaydetmek (loglamak) için kullanılır.

            // Log.Logger: Genellikle Serilog gibi bir loglama kütüphanesini temsil eden vesisteme olay kaydı yapmamızı sağlayan ana nesnedir.

            // .Information: Log kaydının seviyesini belirtir. 'Information' (Bilgi) seviyesi,sistemin normal, beklenen ve başarılı akışlarını kaydetmek için kullanılır.
            // (Başka seviyeler de vardır: Debug, Warning, Error vb.)

            // $"Order Payment Success with OrderId: {@event.OrderId}" : Kaydedilecek metin mesajıdır. Bu mesaj, ödemenin başarılı olduğunu belirtir ve hangi sipariş için başarılı olduğunu(@event.OrderId) gösterir.

            // {@event.OrderId} : Bu ifade, yukarıdaki 'Handle' metoduna OrderPaymentSuccessIntegrationEvent den gelen 'event' nesnesinin (mesajının) içindeki 'OrderId' özelliğinin değerini metin içine yerleştirir.
            // Böylece log kaydı, "Order Payment Success with OrderId: [Gerçek Sipariş Numarası]" şeklinde görünür.

            return Task.CompletedTask;//işlemin tamamlandığını belirtır
        }
    }
}
