using EventBus.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base.Abstraction
{
    public interface IIntegrationEventHandler<TIntegrationEvent> : IIntegrationEventHandler where TIntegrationEvent : IntegrationEvent
    {
        //bu interface gene bu interfaceden İmplemente edilecek nedenle generic yapıda tanımlanıyor
        //dinamik olduğundan  dışardan tip alabilir -TIntegrationEvent-  
        //  where TIntegrationEvent : IntegrationEvent : dışarıdan alınan tip IntegrationEvent tipinden türemiş olmalı kısıtlaması getiriliyor
        Task Handle(TIntegrationEvent @event);
        //handle methodunun amacı event gerçekleştiğinde ne yapılacağını tanımlamak

    }
    public interface IIntegrationEventHandler
    {
        //markup interface :Markup interface, C#'ta içi boş olan, yani hiçbir metot, özellik veya olay tanımlamayan bir arayüzdür.
        //Bu arayüzün amacı, onu uygulayan bir sınıfa davranış değil, sadece bir etiket veya işaret atamaktır.
        //Bu işaret, o sınıf hakkında çalışma zamanında (runtime) ek bilgi sağlar.

    }
    //eventlerin handler edilmesi için bir arayüz tanımlanıyor
    //Bir şey olduğunda (Olay), ne yapacağını (Handler Metodu) sisteme bildirme ve o şey olduğunda bildirilen metodu çalıştırma



}
