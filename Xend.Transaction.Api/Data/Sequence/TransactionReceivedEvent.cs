using Xend.Transaction.Api.Data.Entities;

namespace Xend.Transaction.Api.Data.Sequence
{
    public class TransactionReceivedEvent
    {
        public int ClientId { get; set; }
        public WalletTransaction Transaction { get; set; }
    }
}
