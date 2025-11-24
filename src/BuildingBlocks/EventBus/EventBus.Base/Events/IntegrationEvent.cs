using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using JsonConstructorAttribute = Newtonsoft.Json.JsonConstructorAttribute;

namespace EventBus.Base.Events
{
    public class IntegrationEvent//event ne zaman crete edildi ve her event için ıd değeri atanmalı
    {
        [JsonProperty]// Bu property'nin JSON serileştirme/deserileştirmede kullanılacağını belirtir
        public Guid Id { get; private set; } // Id dışarıdan okunabilir, ama sadece sınıfın içinden set edilebilir

        [JsonProperty]// JSON serileştirme için işaretlenmiş property
        public DateTime CreatedDate { get; private set; } // CreatedDate dışarıdan okunabilir, ama dışarıdan değiştirilemez

        // Parametresiz constructor
        public IntegrationEvent()
        {
            Id = Guid.NewGuid(); // Yeni bir event yaratılırken benzersiz Id atanır
            CreatedDate = DateTime.Now; // evente O anki zaman   yazılır 
        }

        [JsonConstructor] // JSON deserialize sırasında kullanılacak constructor
        public IntegrationEvent(Guid id, DateTime createdDate)
        {
            Id = id; // JSON’dan gelen Id değeri atanır
            CreatedDate = createdDate; // JSON’dan gelen CreatedDate değeri atanır
        }
    }

}
