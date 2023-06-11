using Newtonsoft.Json;
using System.Text;
using Xend.Transaction.Api.Contracts;
using Xend.Transaction.Api.Data.Entities;

namespace Xend.Transaction.Api.Services
{
    public class CryptoApiClient : ICryptoApiClient
    {
        private readonly HttpClient _httpClient;

        public CryptoApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        //public async Task<IList<WalletTransaction>> GetTransactionsAsync(string walletAddress, string currencyType)
        //{
        //    var requestData = new { WalletAddress = walletAddress, CurrencyType = currencyType };
        //    var json = JsonConvert.SerializeObject(requestData);
        //    var content = new StringContent(json, Encoding.UTF8, "application/json");

        //    var response = await _httpClient.PostAsync("api/crypto/transactions", content);
        //    response.EnsureSuccessStatusCode();

        //    var responseContent = await response.Content.ReadAsStringAsync();
        //    var transactions = JsonConvert.DeserializeObject<IList<WalletTransaction>>(responseContent);

        //    return transactions;
        //}

        public List<WalletTransaction> GetTransactionsAsync(string walletAddress, string currencyType)
        {
            // Create a list to hold the sample transactions
            List<WalletTransaction> transactions = new List<WalletTransaction>();

            // Add sample data to the list
            transactions.Add(new WalletTransaction
            {
                TransactionId = 1,
                ClientId = 1,
                WalletAddress = walletAddress,
                CurrencyType = currencyType,
                Timestamp = DateTime.Now,
                Amount = 10.5m,
                CreatedBy = "User1",
                CreatedAt = DateTime.Now,
                UpdatedBy = "User1",
                UpdatedAt = DateTime.Now
            });

            transactions.Add(new WalletTransaction
            {
                TransactionId = 2,
                ClientId = 1,
                WalletAddress = walletAddress,
                CurrencyType = currencyType,
                Timestamp = DateTime.Now,
                Amount = 5.2m,
                CreatedBy = "User2",
                CreatedAt = DateTime.Now,
                UpdatedBy = "User2",
                UpdatedAt = DateTime.Now
            });

            // Add more sample transactions as needed

            // Return the list of sample transactions
            return transactions;
        }
    }
}
