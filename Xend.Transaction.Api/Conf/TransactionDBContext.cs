using Microsoft.EntityFrameworkCore;
using Xend.Transaction.Api.Data.Entities;

namespace Xend.Transaction.Api.Conf
{
    public class TransactionDbContext : DbContext
    {
        public TransactionDbContext(DbContextOptions<TransactionDbContext> options) : base(options)
        {
        }

        public DbSet<WalletTransaction> Transactions { get; set; }
    }
}
