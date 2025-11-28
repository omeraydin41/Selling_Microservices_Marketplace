using CatalogService.App.Core.Domain;
using CatalogService.App.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.App.Infrastructure.EntityConfiguration
{
    class CatalogItemEntityTypeConfiguration : IEntityTypeConfiguration<CatalogItem>
    {
        public void Configure(EntityTypeBuilder<CatalogItem> builder) // CatalogItem varlığı için eşleştirme kurallarını uygulamak üzere EF Core tarafından çağrılır.
        {
            builder.ToTable("Catalog", CatalogContext.DEFAULT_SCHEMA); // Varlığın veritabanında "Catalog" adlı tabloya ve belirtilen şemaya karşılık geldiğini belirtir.

            // --- Id (Birincil Anahtar) Yapılandırması ---
            builder.Property(ci => ci.Id) // Id özelliğinin yapılandırmasına başlar.
                .UseHiLo("catalog_hilo") // Veritabanı kimliği oluşturma stratejisi olarak Hi/Lo algoritmasını kullanır.
                .IsRequired(); // Id alanının veritabanında zorunlu (NOT NULL) olmasını sağlar.

            // --- Name Yapılandırması ---
            builder.Property(ci => ci.Name) // Name (Ad) özelliğinin yapılandırmasına başlar.
                .IsRequired(true) // Name alanının veritabanında zorunlu (NOT NULL) olmasını sağlar.
                .HasMaxLength(50); // Name alanının maksimum uzunluğunu 50 karakter olarak sınırlar.

            // --- Price Yapılandırması ---
            builder.Property(ci => ci.Price) // Price (Fiyat) özelliğinin yapılandırmasına başlar.
                .IsRequired(true); // Price alanının zorunlu (NOT NULL) olmasını sağlar.

            // --- PictureFileName Yapılandırması ---
            builder.Property(ci => ci.PictureFileName) // PictureFileName özelliğinin yapılandırmasına başlar.
                .IsRequired(false); // PictureFileName alanının zorunlu OLMAMASINI (NULLable) sağlar.

            // --- PictureUri Yapılandırması ---
            builder.Ignore(ci => ci.PictureUri); // PictureUri özelliğinin veritabanı tablosuna eşlenmemesini (sadece kod tarafında kullanılmasını) sağlar.

            // --- İlişki (CatalogBrand) Yapılandırması ---
            builder.HasOne(ci => ci.CatalogBrand) // CatalogItem'ın tek bir CatalogBrand'e sahip olduğunu tanımlar (Bire Çok ilişkinin 'Bir' tarafı).
                .WithMany() // CatalogBrand'in CatalogItem'a geri dönüş koleksiyonu OLMADIĞINI (veya anonim olduğunu) belirtir.
                .HasForeignKey(ci => ci.CatalogBrandId); // CatalogBrandId'yi yabancı anahtar (Foreign Key) olarak ayarlar.

            // --- İlişki (CatalogType) Yapılandırması ---
            builder.HasOne(ci => ci.CatalogType) // CatalogItem'ın tek bir CatalogType'a sahip olduğunu tanımlar.
                .WithMany() // CatalogType'ın CatalogItem'a geri dönüş koleksiyonu OLMADIĞINI belirtir.
                .HasForeignKey(ci => ci.CatalogTypeId); // CatalogTypeId'yi yabancı anahtar (Foreign Key) olarak ayarlar.
        }
    }
}
