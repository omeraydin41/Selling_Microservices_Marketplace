using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CatalogService.App.Infrastructure.Context
{
    public class CatalogContextDesingFactory :IDesignTimeDbContextFactory<CatalogContext>
    {
        public CatalogContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CatalogContext>()
            .UseSqlServer("Data Source=c_sqlserver;Initial Catalog=catalog;Persist Security Info=True;User ID=sa;Password=Salih123!");
            return new CatalogContext(optionsBuilder.Options);
        }



    }
}
