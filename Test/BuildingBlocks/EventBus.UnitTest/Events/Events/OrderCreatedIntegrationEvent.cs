using EventBus.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.UnitTest.Events.Events
{
    public class OrderCreatedIntegrationEvent : IntegrationEvent//INTEGRATIONEVENT :event ne zaman crete edildi ve her event için ıd değeri atanmalı
    {
        public int Id { get; set; }

        public OrderCreatedIntegrationEvent(int id)
        {
            Id = id;
        }



    }
}
