using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.SeedWork
{
    // Unit of Work tasarım desenini temsil eden interface
    // IDisposable: işlem bittikten sonra kaynakların temizlenmesini sağlar
    public interface IUnitOfWork : IDisposable
    {
        // Veritabanındaki değişiklikleri asenkron olarak kaydeder
        // Geriye etkilenen kayıt sayısını döner
        // CancellationToken: işlem iptal edilmek istenirse kullanılır
        Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default(CancellationToken)
        );

        // Domain event’leri de dikkate alarak entity’leri kaydeder
        // İşlem başarılı olursa true, olmazsa false döner
        Task<bool> SaveEntitiesAsync(
            CancellationToken cancellationToken = default(CancellationToken)
        );
    }

}
