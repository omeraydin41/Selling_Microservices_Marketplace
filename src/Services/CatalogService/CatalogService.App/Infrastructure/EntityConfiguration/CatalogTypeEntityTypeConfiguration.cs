using CatalogService.App.Core.Domain;
using CatalogService.App.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.App.Infrastructure.EntityConfiguration
{
    class CatalogTypeEntityTypeConfiguration : IEntityTypeConfiguration<CatalogType> // CatalogType varlığı için EF Core eşleştirme yapılandırmasını tanımlar.
    {
        public void Configure(EntityTypeBuilder<CatalogType> builder) // Eşleştirme kurallarını uygulamak için EF Core tarafından çağrılan metot.
        {
            builder.ToTable("CatalogType", CatalogContext.DEFAULT_SCHEMA); // Varlığın veritabanında hangi tabloya ve şemaya karşılık geldiğini belirtir.

            builder.HasKey(ci => ci.Id); // "Id" özelliğini varlığın birincil anahtarı (Primary Key) olarak ayarlar.

            builder.Property(ci => ci.Id) // Id özelliğinin yapılandırmasına başlar.
               .UseHiLo("catalog_type_hilo") // Veritabanı kimliği oluşturma stratejisi olarak Hi/Lo algoritmasını kullanır.
               .IsRequired(); // Id alanının veritabanında zorunlu (NOT NULL) olmasını sağlar.

            builder.Property(cb => cb.Type) // Type özelliğinin yapılandırmasına başlar.
                .IsRequired() // Type alanının veritabanında zorunlu (NOT NULL) olmasını sağlar.
                .HasMaxLength(100); // Type alanının maksimum uzunluğunu 100 karakter olarak sınırlar (örneğin VARCHAR(100)).
        }
    }
}
