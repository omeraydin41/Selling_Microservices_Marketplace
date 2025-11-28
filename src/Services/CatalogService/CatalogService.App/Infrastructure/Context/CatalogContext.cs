using CatalogService.App.Core.Domain;
using CatalogService.App.Infrastructure.EntityConfiguration;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.App.Infrastructure.Context
{
    public class CatalogContext : DbContext //caralogcontext classı bu clastan türemiş olacak
    {
        public const string DEFAULT_SCHEMA = "catalog";

        public CatalogContext(DbContextOptions<CatalogContext> options) : base(options)
        {
        }

        public DbSet<CatalogItem> CatalogItems { get; set; }//ürünlerin tutulduğu class
        public DbSet<CatalogBrand> CatalogBrands { get; set; }//marka bilgilerinin tutulduğu class
        public DbSet<CatalogType> CatalogTypes { get; set; }//ürün tiplerinin tutulduğu class


        protected override void OnModelCreating(ModelBuilder builder)//tablo ozelliklerini belirlediğimiz method bu de entıtıyconf classları ıcınde yapıyoruz
        {
            builder.ApplyConfiguration(new CatalogBrandEntityTypeConfiguration());
            builder.ApplyConfiguration(new CatalogItemEntityTypeConfiguration());
            builder.ApplyConfiguration(new CatalogTypeEntityTypeConfiguration());

        }


    }
}
