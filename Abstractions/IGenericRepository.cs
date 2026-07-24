using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Abstractions
{
    public interface IGenericRepository<T> where T : class
    {
        void Add(T entity);
        void AddRange(List<T> entities);

        void Remove(T entity);
        void RemoveRange(List<T> entities);

        void Update(T entity);

        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

        Task<List<T>> GetAllAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            bool ignoreTenantFilter = false
        );

        Task<T?> GetSingleAsync(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            bool tracking = false,
            bool ignoreTenantFilter = false
        );

        Task<T?> GetByKeyIdAsync(
            long keyId,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            bool tracking = false,
            bool ignoreTenantFilter = false
        );

        Task<T?> GetByIdAsync(int id);

        Task<bool> SaveChangesAsync();
    }
}
