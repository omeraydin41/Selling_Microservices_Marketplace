using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.SeedWork
{
    // Tüm entity (veritabanı nesneleri) sınıflarının miras alacağı temel sınıf
    public abstract class BaseEntity
    {
        // Her entity için benzersiz Id alanı
        // virtual: İstersek alt sınıflarda override edebiliriz
        // protected set: Id sadece sınıf içinden veya miras alan sınıflardan değiştirilebilir
        public virtual Guid Id { get; protected set; }

        // Kaydın oluşturulma tarihini tutar
        public DateTime CreateDate { get; set; }

        // HashCode hesaplandı mı diye kontrol etmek için kullanılan alan
        // nullable (int?) çünkü ilk başta değer yok
        int? _requestedHashCode;

        // Domain event’leri tutmak için liste
        // private yapıldı çünkü dışarıdan direkt değiştirilmesi istenmiyor
        private List<INotification> domainEvents;

        // Domain event’leri sadece okunabilir şekilde dışarı açıyoruz
        public IReadOnlyCollection<INotification> DomainEvents => domainEvents?.AsReadOnly();

        // Yeni bir domain event eklemek için kullanılan metot
        public void AddDomainEvent(INotification eventItem)
        {
            // Eğer liste daha önce oluşturulmadıysa burada oluşturulur
            domainEvents = domainEvents ?? new List<INotification>();

            // Event listeye eklenir
            domainEvents.Add(eventItem);
        }

        // Var olan bir domain event’i listeden silmek için kullanılan metot
        public void RemoveDomainEvent(INotification eventItem)
        {
            domainEvents?.Remove(eventItem);
        }

        // Tüm domain event’leri temizlemek için kullanılan metot
        public void ClearDomainEvents()
        {
            domainEvents?.Clear();
        }

        // Entity henüz veritabanına kaydedilmiş mi diye kontrol eder
        // Id default ise (Guid.Empty) transient kabul edilir
        public bool IsTransient()
        {
            return Id == default;
        }

        // Nesnelerin eşitliğini kontrol etmek için override edilen Equals metodu
        public override bool Equals(object obj)
        {
            // Eğer karşılaştırılan nesne null ise eşit değildir
            if (obj == null || !(obj is BaseEntity))
                return false;

            // Aynı referansa sahiplerse kesinlikle eşittir
            if (ReferenceEquals(this, obj))
                return true;

            // Tipleri farklıysa eşit sayılmaz
            if (GetType() != obj.GetType())
                return false;

            // Objeyi BaseEntity tipine çeviriyoruz
            BaseEntity item = (BaseEntity)obj;

            // Eğer iki nesneden biri transient ise karşılaştırma yapılmaz
            if (item.IsTransient() || IsTransient())
                return false;
            else
                // Id’leri aynıysa entity’ler eşittir
                return item.Id == Id;
        }

        // Hash tabanlı koleksiyonlar (Dictionary, HashSet) için kullanılan metot
        public override int GetHashCode()
        {
            // Eğer entity veritabanına kaydedilmişse
            if (!IsTransient())
            {
                // HashCode daha önce hesaplanmadıysa hesapla
                if (!_requestedHashCode.HasValue)
                    // XOR işlemi ile daha dengeli bir hash dağılımı sağlanır
                    _requestedHashCode = Id.GetHashCode() ^ 31;

                return _requestedHashCode.Value;
            }
            else
                // Eğer transient ise base sınıfın hash kodu kullanılır
                return base.GetHashCode();
        }

        // == operatörünü override ediyoruz
        public static bool operator ==(BaseEntity left, BaseEntity right)
        {
            // Eğer sol taraf null ise sağ taraf da null mı kontrol edilir
            if (Equals(left, null))
                return Equals(right, null) ? true : false;
            else
                return left.Equals(right);
        }

        // != operatörünü override ediyoruz
        public static bool operator !=(BaseEntity left, BaseEntity right)
        {
            return !(left == right);
        }
    }

}
