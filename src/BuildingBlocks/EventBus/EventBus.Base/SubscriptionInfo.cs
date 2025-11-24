using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base //nehirsulatte
{
    public class SubscriptionInfo // Genel bir sınıf tanımlamasıdır. Abonelik bilgilerini taşıyacak veri modelidir.
    {
        public Type HandlerType { get; } // Abone olan işleyici sınıfın tipini (örneğin MyHandler) tutan, sadece okunabilir özelliktir.

        public SubscriptionInfo(Type handlerType) // Sınıfın kurucu (constructor) metodudur. Bir 'Type' nesnesi alması zorunludur.
        {
            // Gelen 'handlerType' null (boş) ise 'ArgumentNullException' fırlatır, değilse atama yapar.
            HandlerType = handlerType ?? throw new ArgumentNullException(nameof(handlerType));
        }

        // burdaki amaç event handler ın tipini tutmak (Bu, sınıfın temel amacını özetleyen bir yorumdur.)

        public static SubscriptionInfo Typed(Type handlerType) // Abonelik nesnesini oluşturmak için kullanılan yardımcı (Fabrika) metottur.
        {
            return new SubscriptionInfo(handlerType); // Yeni bir SubscriptionInfo nesnesi oluşturup, içine handler tipini koyarak döndürür.
        }
    }
    /*Bu C# sınıfı (SubscriptionInfo), Event Bus (Olay Veri Yolu) yapılarında bir olaya kimin abone olduğunu kaydetmek 
     * ve izlemek için kullanılan temel bir veri taşıyıcısıdır.
     Esas amacı, belirli bir olayı işleyecek olan Abone (Handler) tipini güvenli bir şekilde saklamaktır.*/
}
