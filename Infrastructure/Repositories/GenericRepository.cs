using Abstractions;
using Domain.Interfaces;
using IdGen;
using Infrastructure.Data;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, IBaseEntity, ISoftDeletable
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;
        private readonly IIdGenerator<long> _idGenerator;
        private readonly IUserContext _userContext;
        public GenericRepository(AppDbContext context, IIdGenerator<long> idGenerator, IUserContext userContext)
        {
            _context = context;
            _dbSet = _context.Set<T>();
            _idGenerator = idGenerator;
            _userContext = userContext;
        }

        public void Add(T entity)
        {
            entity.KeyId = _idGenerator.CreateId();
            entity.ApplyFullAudit(AuditExtension.AuditAction.Create, _userContext.UserId);
            _dbSet.Add(entity);
        }

        public void AddRange(List<T> entities)
        {
            foreach (var entity in entities)
            {
                entity.KeyId = _idGenerator.CreateId();
                entity.ApplyFullAudit(AuditExtension.AuditAction.Create, _userContext.UserId);
            }
            _dbSet.AddRange(entities);
        }

        public void Remove(T entity)
        {
            entity.IsDeleted = true;
            entity.ApplyFullAudit(AuditExtension.AuditAction.Delete, _userContext.UserId);
        }

        public void RemoveRange(List<T> entities)
        {
            foreach (var entity in entities)
            {
                entity.IsDeleted = true;
                entity.ApplyFullAudit(AuditExtension.AuditAction.Delete, _userContext.UserId);
            }
            _dbSet.RemoveRange(entities);
        }

        public void Update(T entity)
        {
            entity.ApplyFullAudit(AuditExtension.AuditAction.Update, _userContext.UserId);
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.IgnoreQueryFilters().AnyAsync(predicate);
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<List<T>> GetAllAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            bool ignoreTenantFilter = false
        )
        {
            IQueryable<T> query = _dbSet.AsNoTracking().Where(predicate ?? (_ => true));

            if (include != null)
            {
                query = include(query);
            }
            if (!ignoreTenantFilter && typeof(ITenantEntity).IsAssignableFrom(typeof(T)))
            {
                ApplyTenantFilter(query);
            }

            return await query.ToListAsync();
        }

        public async Task<T?> GetByKeyIdAsync(
            long keyId,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            bool tracking = false,
            bool ignoreTenantFilter = false
        )
        {
            IQueryable<T> query = _dbSet;

            if (!tracking)
            {
                query.AsNoTracking();
            }

            if (!ignoreTenantFilter && typeof(ITenantEntity).IsAssignableFrom(typeof(T)))
            {
                ApplyTenantFilter(query);
            }

            query = query.Where(e => e.KeyId == keyId);
            if (include != null)
            {
                query = include(query);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<T?> GetSingleAsync(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            bool tracking = false,
            bool ignoreTenantFilter = false
        )
        {
            IQueryable<T> query = _dbSet;
            if (!tracking)
            {
                query = query.AsNoTracking();
            }

            if (!ignoreTenantFilter && typeof(ITenantEntity).IsAssignableFrom(typeof(T)))
            {
                ApplyTenantFilter(query);
            }

            query = query.Where(predicate);
            if (include != null)
            {
                query = include(query);
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        private IQueryable<T> ApplyTenantFilter(IQueryable<T> query)
        {
            if (typeof(ITenantEntity).IsAssignableFrom(typeof(T)))
            {
                var parameter = Expression.Parameter(typeof(T), "e");
                var property = Expression.Property(Expression.Convert(parameter, typeof(ITenantEntity)), nameof(ITenantEntity.OrganizationId));
                var constant = Expression.Constant(_userContext.OrganizationId);
                var equality = Expression.Equal(property, constant);
                var lambda = Expression.Lambda<Func<T, bool>>(equality, parameter);
                query = query.Where(lambda);
            }

            return query;
        }
    }
}
