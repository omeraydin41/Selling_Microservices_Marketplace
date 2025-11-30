using EventBus.Base.Abstraction;
using NotificationService.IntegrationEvents.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Serilog;

namespace NotificationService.IntegrationEvents.EventHandlers
{
    class OrderPaymentFailedIntegrationEventHandler : IIntegrationEventHandler<OrderPaymentFailedIntegrationEvent>
    //OrderPaymentFailedIntegrationEvent I ıntegratıon evetten kalıtım aldı : burda IIntegrationEventHandler ınterfacesinden klaıtım aldı 
    {
        public Task Handle(OrderPaymentFailedIntegrationEvent @event)//OrderPaymentFailedIntegrationEvent turundehi methodu handle(işle) yapar async 
        // Bu metot, 'Handle' (İşle/Ele Al) adını taşır ve 'public Task' döndürür.'Task' döndürmesi, bu işlemin asenkron (eş zamanlı olmayan) bir işlem olduğunu ve arka planda çalışabileceğini belirtir.
        {
            // Send Fail Notification (Sms, EMail, Push)
            // Bu, geliştiricinin buraya yapılması gereken işi not ettiği bir yorumdur (gerçek bir kod değildir).
            // Amaç: Ödeme başarısız olduğunda, kullanıcıya SMS, E-posta veya mobil bildirim (Push) gibi yollarla
            // bir başarısızlık bildirimi gönderme işleminin bu kısma geleceğini belirtir.

            Log.Logger.Information($"Order Payment failed with OrderId: {@event.OrderId}, ErrorMessage: {@event.ErrorMessage}");
            // Bu satır, sistemi izlemek ve hata ayıklamak için çok önemlidir (Loglama).'Log.Logger' genellikle bir loglama kütüphanesini (örneğin Serilog) temsil eder.
            // 'Information' seviyesinde bir kayıt oluşturur. Kayıt mesajında: "Order Payment failed..." metnini yazar ve gelen '@event' nesnesinin
            // içindeki 'OrderId' ve 'ErrorMessage' değerlerini dinamik olarak mesajın içine ekler. Bu sayede geliştirici, hangi siparişte ne hatanın oluştuğunu kolayca görebilir.

            return Task.CompletedTask;
            // Bu satır, metot bir 'Task' döndürmek zorunda olduğu için kullanılır.'Task.CompletedTask', işleyicinin (Handler) yapması gereken tüm işlemleri
            // (bu örnekte sadece loglama ve varsayılan bildirim yorumu) bitirdiğini ve asenkron işlemin başarılı bir şekilde tamamlandığını bildirir.
        }
    }
}
