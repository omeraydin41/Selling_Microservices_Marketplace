using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventBus.Base.Events;

namespace OrderService.Application.IntegraıonEvents
{
    // Order işlemi başladığında diğer servisleri bilgilendirmek için kullanılan Integration Event
    // IntegrationEvent’ten türetilmiştir
    public class OrderStartedIntegrationEvent : IntegrationEvent
    {
        // Siparişi başlatan kullanıcının adı
        public string UserName { get; set; }

        // Başlatılan siparişin benzersiz Id değeri
        public Guid OrderId { get; set; }

        // Constructor
        // Event oluşturulurken gerekli bilgileri set eder
        public OrderStartedIntegrationEvent(string userName, Guid orderId)
        {
            // Kullanıcı adı atanır
            UserName = userName;

            // Sipariş Id değeri atanır
            OrderId = orderId;
        }
    }

}
