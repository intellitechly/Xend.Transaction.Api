using System;
using System.Threading.Tasks;
using Serilog;
using RabbitMQ;
using Xend.Transaction.Api.Contracts;

using Xend.Transaction.Api.Data.Sequence;
using Microsoft.Extensions.Caching.Memory;
using Xend.Transaction.Api.Conf;
using Xend.Transaction.Api.Data.Entities;
using Xend.Transaction.Api.PreCompiledQ;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using Microsoft.EntityFrameworkCore;

namespace Xend.Transaction.Api.Services
{


    public class TransactionsService : ITransactionService
    {
        private readonly ICryptoApiClient _cryptoApiClient;
        private readonly IMessageBroker _messageBroker;
        private readonly IEventBus _eventBus;
       
        private readonly ILogger _logger;
        private readonly IMemoryCache _cache;
        private readonly TransactionDbContext _dbContext;
        public TransactionsService(
            ICryptoApiClient cryptoApiClient,
            IMessageBroker messageBroker,
            IEventBus eventBus,
           
            TransactionDbContext dbContext,
            IMemoryCache cache,
            ILogger<TransactionsService> logger)
        {
            _cryptoApiClient = cryptoApiClient;
            _messageBroker = messageBroker;
            _eventBus = eventBus;
            _dbContext = dbContext;
            _cache = cache;
         
            _logger = (ILogger?)logger;
        }

        public async Task UpdateTransactions(UpdateTransactionsCommand command)
        {
            try
            {
                // Check if the transaction update request is unique
                if (IsDuplicateTransactionRequest(command))
                {
                    await Console.Out.WriteLineAsync("Duplicate transaction update request received. Ignoring.");
                    //_logger.Information();
                    return;
                }

                // Query the crypto API for transactions
                var transactions =  _cryptoApiClient.GetTransactionsAsync(command.WalletAddress, command.CurrencyType);

                // Check if new transactions were received
                if (transactions.Count > 0)
                {
                    // Process the new transactions
                    foreach (var transaction in transactions)
                    {
                        // Publish a TransactionReceived event to the event bus
                        var transactionReceivedEvent = new TransactionReceivedEvent
                        {
                            ClientId = command.ClientId,
                            Transaction = transaction
                        };
                        _eventBus.Publish(transactionReceivedEvent);

                        // Save the transaction in the database
                        SaveTransaction(transaction);
                    }
                }
            }
            catch (Exception ex)
            {
              
                throw; // Rethrow the exception for error handling at a higher level
            }
        }

        public bool IsDuplicateTransactionRequest(UpdateTransactionsCommand command)
        {
            // Generate a cache key based on the command parameters
            var cacheKey = GenerateCacheKey(command);

            // Try to get the value from the cache
            if (_cache.TryGetValue(cacheKey, out bool isDuplicate))
            {
                // If the value exists in the cache, return it
                return isDuplicate;
            }

            // If the value is not found in the cache, perform the duplicate check
            var existingTransaction = GetTransactionByClientIdWalletAndCurrency(command.ClientId, command.WalletAddress, command.CurrencyType);

            // Determine if it is a duplicate request
            isDuplicate = existingTransaction != null;

            // Add the result to the cache with a sliding expiration of 5 minutes
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));
            _cache.Set(cacheKey, isDuplicate, cacheEntryOptions);

            // Return the result
            return isDuplicate;
        }

        private string GenerateCacheKey(UpdateTransactionsCommand command)
        {
            // Generate a unique cache key based on the command parameters
            return $"{command.ClientId}_{command.WalletAddress}_{command.CurrencyType}";
        }

        public  async Task<WalletTransaction> GetTransactionsByClientIdAsync( int clientId)
        {
            var model = _dbContext.Transactions.Where(x => x.ClientId == clientId).FirstOrDefault();
            return model;
        }

        public void SaveTransaction(WalletTransaction transaction)
        {
            _dbContext.Transactions.Add(transaction);
             _dbContext.SaveChangesAsync();
        }

        public async Task<WalletTransaction> GetTransactionByClientIdWalletAndCurrency(int clientId, string walletAddress, string currencyType)
        {
            return await _dbContext.Transactions
                .FirstOrDefaultAsync(t =>
                    t.ClientId == clientId &&
                    t.WalletAddress == walletAddress &&
                    t.CurrencyType == currencyType);
        }

        public bool DoesTransactionExist(int transactionId)
        {
            return  _dbContext.Transactions.Any(t => t.TransactionId == transactionId);

        }

        WalletTransaction ITransactionService.GetTransactionByClientIdWalletAndCurrency(int clientId, string walletAddress, string currencyType)
        {
            return  _dbContext.Transactions
               .FirstOrDefault(t =>
                   t.ClientId == clientId &&
                   t.WalletAddress == walletAddress &&
                   t.CurrencyType == currencyType);
        }

     
    }

}
