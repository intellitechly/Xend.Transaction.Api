using Xend.Transaction.Api.Data.Entities;

namespace Xend.Transaction.Api.Contracts
{
    public interface ICryptoApiClient
    {
     List<WalletTransaction> GetTransactionsAsync(string walletAddress, string currencyType);

    }
}
