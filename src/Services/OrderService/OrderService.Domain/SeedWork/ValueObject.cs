using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.SeedWork
{
    // DDD (Domain Driven Design) yaklaşımında kullanılan Value Object için temel sınıf
    // Value Object’ler kimlik (Id) ile değil, değerleriyle karşılaştırılır
    public abstract class ValueObject
    {
        // == operatörü için kullanılan yardımcı metot
        protected static bool EqualOperator(ValueObject left, ValueObject right)
        {
            // Sadece biri null ise eşit değildir
            if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
            {
                return false;
            }

            // İkisi de null ise true, değilse Equals ile karşılaştır
            return ReferenceEquals(left, null) || left.Equals(right);
        }

        // != operatörü için kullanılan yardımcı metot
        protected static bool NotEqualOperator(ValueObject left, ValueObject right)
        {
            return !EqualOperator(left, right);
        }

        // Value Object’in eşitliğini belirleyen alanları döndürür
        // Alt sınıflar hangi alanların karşılaştırılacağını burada belirtir
        protected abstract IEnumerable<object> GetEqualityComponents();

        // Nesnelerin eşitliğini değerlerine göre kontrol eder
        public override bool Equals(object obj)
        {
            // Null ise veya tipler farklıysa eşit değildir
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            // Objeyi ValueObject tipine çeviriyoruz
            var other = (ValueObject)obj;

            // Tüm equality component’ler sırayla aynı mı kontrol edilir
            return GetEqualityComponents()
                .SequenceEqual(other.GetEqualityComponents());
        }

        // Hash tabanlı koleksiyonlar için hash kodu üretir
        public override int GetHashCode()
        {
            return GetEqualityComponents()
                // Null olanlar için 0, dolu olanlar için kendi hash değeri alınır
                .Select(x => x != null ? x.GetHashCode() : 0)
                // XOR işlemi ile tek bir hash kod üretilir
                .Aggregate((x, y) => x ^ y);
        }

        // Value Object’in kopyasını oluşturur
        // MemberwiseClone: nesnenin shallow copy’sini alır
        public ValueObject GetCopy()
        {
            return MemberwiseClone() as ValueObject;
        }
    }

}
