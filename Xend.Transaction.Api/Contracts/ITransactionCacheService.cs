namespace Xend.Transaction.Api.Contracts
{
    public interface ITransactionCacheService
    {
        bool IsTransactionCached(string cacheKey);
        void CacheTransaction(string cacheKey);
    }


}
