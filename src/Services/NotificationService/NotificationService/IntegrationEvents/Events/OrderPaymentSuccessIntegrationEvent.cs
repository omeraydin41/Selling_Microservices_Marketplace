using EventBus.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.IntegrationEvents.Events
{
    public class OrderPaymentSuccessIntegrationEvent : IntegrationEvent
    {
        // Bu, sistemin diğer parçalarına 'ödeme başarılı oldu' bilgisini ileten bir mesaj sınıfıdır.
      
        public Guid OrderId { get; }
        // 'public Guid OrderId' demek, bu başarılı ödeme mesajının taşıyacağı temel verinin siparişin benzersiz kimliği (ID) olduğunu belirtir.'Guid' (Global Unique Identifier) benzersiz numara formatıdır.
        // '{ get; }' sadece okunabilir bir özellik olduğunu, yani nesne oluştuktan sonra bu değerin değiştirilemeyeceğini belirtir (Immutable - Değişmez).

        public OrderPaymentSuccessIntegrationEvent(Guid orderId) => OrderId = orderId;
        // Bu, sınıfın 'yapıcı metodu' (constructor) denilen özel metodudur.Bu mesaj nesnesi oluşturulurken, parantez içinde bir 'orderId' değeri verilmek zorundadır.
        // '=> OrderId = orderId;' ise C#'a özgü kısa bir yazım şeklidir (expression body).Anlamı: Yapıcı metot çağrıldığında, dışarıdan gelen 'orderId' değerinisınıfın kendi 'OrderId' özelliğine hemen ata.
        
    }
    
}
