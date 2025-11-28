using CatalogService.App.Core.Domain;
using CatalogService.App.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.App.Infrastructure.EntityConfiguration
{
    class CatalogBrandEntityTypeConfiguration : IEntityTypeConfiguration<CatalogBrand>//catalogbrand classı IEntityTypeConfiguration gömulu classından türemiş olacak
    {
        public void Configure(EntityTypeBuilder<CatalogBrand> builder) // CatalogBrand varlığı için eşleştirme kurallarını uygulamak üzere EF Core tarafından çağrılır.
        {
            builder.ToTable("CatalogBrand", CatalogContext.DEFAULT_SCHEMA); // Varlığın veritabanında "CatalogBrand" adlı tabloya ve belirtilen şemaya karşılık geldiğini belirtir.

            // --- Id (Birincil Anahtar) Yapılandırması ---
            builder.HasKey(ci => ci.Id); // "Id" özelliğini varlığın birincil anahtarı (Primary Key) olarak ayarlar.
                                         // // (Sizin yorumunuz: brand özelliğini primary key olarak belirledik -> Bu, 'Id' özelliğini belirler.)

            builder.Property(ci => ci.Id) // Id özelliğinin yapılandırmasına başlar.
               .UseHiLo("catalog_brand_hilo") // Veritabanı kimliği oluşturma stratejisi olarak Hi/Lo algoritmasını kullanır.
                                              // // (Sizin yorumunuz: hilo stratejisini kullanarak id olusturulacak)
               .IsRequired(); // Id alanının veritabanında zorunlu (NOT NULL) olmasını sağlar.

            // --- Brand Adı Yapılandırması ---
            builder.Property(cb => cb.Brand) // Brand (Marka Adı) özelliğinin yapılandırmasına başlar.
                                             // // (Sizin yorumunuz: brand özelliğinin yapılandırmasına başlar)
                .IsRequired() // Brand alanının veritabanında zorunlu (NOT NULL) olmasını sağlar.
                .HasMaxLength(100); // Brand alanının maksimum uzunluğunu 100 karakter olarak sınırlar (örneğin VARCHAR(100)).
        }
    }
}
