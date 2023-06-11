
using Xend.Transaction.Api.Conf;
using Xend.Transaction.Api.Data.Entities;

namespace Xend.Transaction.Api.PreCompiledQ
{
    public class TransactionQueries
    {
        public static readonly Func<TransactionDbContext, int, Task<WalletTransaction>> GetTransactionsByClientIdQuery =
         Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery((TransactionDbContext dbContext, int clientId) =>
             dbContext.Transactions.Where(x => x.ClientId == clientId).FirstOrDefault());

       

    }
}
