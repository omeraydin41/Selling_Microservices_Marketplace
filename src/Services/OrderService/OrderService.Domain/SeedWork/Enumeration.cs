using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.SeedWork
{
    // Klasik enum yapısının daha gelişmiş hali gibi çalışan abstract sınıf
    // Karşılaştırma yapılabilmesi için IComparable interface’ini implemente eder
    public abstract class Enumeration : IComparable
    {
        // Enum benzeri yapının ekranda gösterilecek adı
        public string Name { get; private set; }

        // Enum benzeri yapının sayısal değeri
        public int Id { get; private set; }

        // Constructor
        // Id ve Name değerleri burada set edilir
        // protected: sadece bu sınıf ve miras alan sınıflar kullanabilir
        protected Enumeration(int id, string name) => (Id, Name) = (id, name);

        // Nesne string olarak yazdırıldığında Name dönsün diye override edilir
        public override string ToString() => Name;

        // T tipindeki tüm Enumeration değerlerini reflection kullanarak getirir
        public static IEnumerable<T> GetAll<T>() where T : Enumeration =>
            typeof(T)
                // Public, static ve sadece ilgili sınıfta tanımlı alanları alır
                .GetFields(BindingFlags.Public |
                           BindingFlags.Static |
                           BindingFlags.DeclaredOnly)
                // Alanların değerlerini alır
                .Select(f => f.GetValue(null))
                // Tip dönüşümü yapar
                .Cast<T>();

        // İki Enumeration nesnesinin eşitliğini kontrol eder
        public override bool Equals(object obj)
        {
            // Eğer obje Enumeration değilse false döner
            if (obj is not Enumeration otherValue)
            {
                return false;
            }

            // Tipleri aynı mı kontrol edilir
            var typeMatches = GetType().Equals(obj.GetType());

            // Id değerleri aynı mı kontrol edilir
            var valueMatches = Id.Equals(otherValue.Id);

            // Hem tip hem Id aynıysa eşittir
            return typeMatches && valueMatches;
        }

        // Hash tabanlı koleksiyonlarda kullanılmak üzere override edilir
        public override int GetHashCode() => Id.GetHashCode();

        // İki Enumeration değeri arasındaki mutlak farkı hesaplar
        public static int AbsoluteDifference(Enumeration firstValue, Enumeration secondValue)
        {
            // Id’ler arasındaki farkın mutlak değeri alınır
            var absoluteDifference = Math.Abs(firstValue.Id - secondValue.Id);
            return absoluteDifference;
        }

        // Id değerine göre Enumeration nesnesi döner
        public static T FromValue<T>(int value) where T : Enumeration
        {
            // Id’si verilen değere eşit olan Enumeration bulunur
            var matchingItem = Parse<T, int>(value, "value", item => item.Id == value);
            return matchingItem;
        }

        // Name (ekranda görünen isim) değerine göre Enumeration nesnesi döner
        public static T FromDisplayName<T>(string displayName) where T : Enumeration
        {
            // Name’i verilen stringe eşit olan Enumeration bulunur
            var matchingItem = Parse<T, string>(displayName, "display name", item => item.Name == displayName);
            return matchingItem;
        }

        // Ortak parse işlemini yapan private metot
        // Predicate sayesinde arama kriteri dışarıdan verilir
        private static T Parse<T, K>(K value, string description, Func<T, bool> predicate)
            where T : Enumeration
        {
            // T tipindeki tüm Enumeration değerleri arasında arama yapılır
            var matchingItem = GetAll<T>().FirstOrDefault(predicate);

            // Eğer eşleşen değer bulunamazsa hata fırlatılır
            if (matchingItem == null)
                throw new InvalidOperationException(
                    $"'{value}' is not a valid {description} in {typeof(T)}"
                );

            return matchingItem;
        }

        // Enumeration nesnelerini Id değerine göre karşılaştırmak için kullanılır
        public int CompareTo(object other) => Id.CompareTo(((Enumeration)other).Id);
    }

}
