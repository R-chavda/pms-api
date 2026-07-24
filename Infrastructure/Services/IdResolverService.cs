using Abstractions;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using static Domain.Wrappers.AppException;

namespace Infrastructure.Services
{
    public class IdResolverService : IIdResolverService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<IdResolverService> _logger;
        private readonly IUserContext _userContext;
        //private readonly IDistributedCache _cache;

        public IdResolverService(
            AppDbContext context,
            IUserContext userContext,
            ILogger<IdResolverService> logger)
        {
            _context = context;
            _userContext = userContext;
            //_cache = cache;
            _logger = logger;
        }

        public async Task<int> ResolveIdAsync<TEntity>(string keyId) where TEntity : class, IBaseEntity
        {
            _logger.LogInformation($"Resolving ID for {typeof(TEntity).Name} with KeyId: {keyId}");

            if (string.IsNullOrWhiteSpace(keyId) || !long.TryParse(keyId, out long parsedKeyId))
                throw new BadRequestException("Invalid or missing KeyId.");

            //var cacheKey = $"{typeof(TEntity).Name.ToLower()}:{parsedKeyId}";
            //var cachedId = await _cache.GetStringAsync(cacheKey);

            //if (cachedId != null && int.TryParse(cachedId, out int resolvedId))
            //{
            //    _logger.LogInformation($"Cache hit for {cacheKey} -> {resolvedId}");
            //    return resolvedId;
            //}

            var id = await _context.Set<TEntity>()
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(e => e.KeyId == parsedKeyId)
                .Select(e => e.Id)
                .FirstOrDefaultAsync();

            if (id == 0)
                throw new NotFoundException($"{typeof(TEntity).Name} not found for KeyId: {keyId}");

            //await _cache.SetStringAsync(cacheKey, id.ToString(), new DistributedCacheEntryOptions
            //{
            //    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            //});

            //_logger.LogInformation($"Cache set for {cacheKey} -> {id}");

            return id;
        }

        public async Task<int?> ResolveOptionalIdAsync<TEntity>(string? keyId) where TEntity : class, IBaseEntity
        {
            if (string.IsNullOrWhiteSpace(keyId))
                return null;

            if (!long.TryParse(keyId, out long parsedKeyId))
                throw new BadRequestException($"Invalid KeyId format: {keyId}");

            var id = await _context.Set<TEntity>()
                .AsNoTracking()
                .Where(e => e.KeyId == parsedKeyId)
                .Select(e => e.Id)
                .FirstOrDefaultAsync();

            return id == 0 ? null : id;
        }
    }
}
