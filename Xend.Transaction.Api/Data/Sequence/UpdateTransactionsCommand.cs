namespace Xend.Transaction.Api.Data.Sequence
{
    public class UpdateTransactionsCommand
    {
        public int ClientId { get; set; }
        public string WalletAddress { get; set; }
        public string CurrencyType { get; set; }
    }
}
