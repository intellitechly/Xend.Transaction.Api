using System.ComponentModel.DataAnnotations;

namespace Xend.Transaction.Api.Data.Entities
{
    public class WalletTransaction
    {
        [Key]
        public int TransactionId { get; set; }
        public int ClientId { get; set; }
        public string WalletAddress { get; set; }
        public string CurrencyType { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal Amount { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
 
    }
}
