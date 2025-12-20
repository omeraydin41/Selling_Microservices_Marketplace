using OrderService.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.Interfaces.Repositories
{
    // Generic Repository yapısını temsil eden interface
    // T: BaseEntity’den türeyen herhangi bir entity olabilir
    public interface IGenericRepository<T> : IRepository<T> where T : BaseEntity
    {
        // T tipindeki tüm kayıtları veritabanından getirir
        Task<List<T>> GetAll();

        // Filtre, sıralama ve include (ilişkili tablolar) alabilen gelişmiş get metodu
        // filter: where şartı
        // orderBy: sıralama işlemi
        // includes: navigation property’leri dahil etmek için kullanılır
        Task<List<T>> Get(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            params Expression<Func<T, object>>[] includes
        );

        // Filtre ve include alabilen daha basit get metodu
        Task<List<T>> Get(
            Expression<Func<T, bool>> filter = null,
            params Expression<Func<T, object>>[] includes
        );

        // Id değerine göre tek bir entity getirir
        Task<T> GetById(Guid id);

        // Id’ye göre entity getirir ve istenen navigation property’leri include eder
        Task<T> GetByIdAsync(
            Guid id,
            params Expression<Func<T, object>>[] includes
        );

        // Verilen koşula uyan tek bir entity döner
        // Genelde FirstOrDefault gibi çalışır
        Task<T> GetSingleAsync(
            Expression<Func<T, bool>> expression,
            params Expression<Func<T, object>>[] includes
        );

        // Yeni bir entity’yi veritabanına ekler
        Task<T> AddAsync(T entity);

        // Var olan entity’yi günceller
        T Update(T entity);
    }

}
