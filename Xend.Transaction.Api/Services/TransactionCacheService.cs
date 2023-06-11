using Microsoft.Extensions.Caching.Memory;
using Xend.Transaction.Api.Contracts;

namespace Xend.Transaction.Api.Services
{
    public class TransactionCacheService : ITransactionCacheService
    {
        private readonly IMemoryCache _cache;

        public TransactionCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public bool IsTransactionCached(string cacheKey)
        {
            return _cache.TryGetValue(cacheKey, out _);
        }

        public void CacheTransaction(string cacheKey)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));
            _cache.Set(cacheKey, true, cacheEntryOptions);
        }
    }
}
