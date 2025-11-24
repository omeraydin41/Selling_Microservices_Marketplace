using EventBus.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base.Abstraction
{
    public interface IEventBus
    {
        void publish(IntegrationEvent @event);//bir event alır ve yayınlar 
        //publish methodu event bus a event gönderir integration event türünde bir @event değişkeni alır
        void Subscribe<T, TH>()//bize IntegrationEvent ve IIntegrationEventHandler verecek ve bız de bunu rabbit e bildireceğiz
            where T : IntegrationEvent //T tipi IntegrationEvent türünden türemiş olmalı
            where TH : IIntegrationEventHandler<T>; //TH tipi IIntegrationEventHandler<T> türünden türemiş olmalı
        void UnSubscribe<T, TH>()//bir bileşenin artık belirli bir olayı dinlemeyi bıraktığını Event Bus'a bildirdiği işlemdir.
            where T : IntegrationEvent//T tipi IntegrationEvent türünden türemiş olmalı
            where TH : IIntegrationEventHandler<T>;//TH tipi IIntegrationEventHandler<T> türünden türemiş olmalı
        //method voıd olması event publish edildikten sonra gerisiniin önemsiz olduğu : fire and forget anlamına gelir
    }
    //publish : event yayınlama : belirli bir eventin gerçekleştiğini sistemde duyurma event bus a event gönderme
    //subscribe : event abone olma
    //UnSubscribe : event aboneliğini iptal etme
    //void : o metodun işini yaptıktan sonra çağrıldığı yere hiçbir veri veya sonuç döndürmeyeceği anlamına gelir.
}
