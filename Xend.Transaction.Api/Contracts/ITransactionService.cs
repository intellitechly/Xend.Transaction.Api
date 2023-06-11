

using Xend.Transaction.Api.Data.Entities;

namespace Xend.Transaction.Api.Contracts
{
    public interface ITransactionService
    {
        void SaveTransaction(WalletTransaction transaction);
        WalletTransaction GetTransactionByClientIdWalletAndCurrency(int clientId, string walletAddress, string currencyType);
        bool DoesTransactionExist(int transactionId);
    }
}
