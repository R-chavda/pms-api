using Domain.Interfaces;

namespace Abstractions
{
    public interface IIdResolverService
    {
        Task<int> ResolveIdAsync<TEntity>(string keyId) where TEntity : class, IBaseEntity;
        Task<int?> ResolveOptionalIdAsync<TEntity>(string? keyId) where TEntity : class, IBaseEntity;
    }
}
