using Microsoft.AspNetCore.Mvc;
using Xend.Transaction.Api.Conf;
using Xend.Transaction.Api.Data.Entities;
using Xend.Transaction.Api.Data.Sequence;
using Xend.Transaction.Api.Handlers;
using Xend.Transaction.Api.Services;

namespace Xend.Transaction.Api.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly TransactionsService _transactionService;
        private readonly TransactionDbContext _dbContext;
        public TransactionsController(TransactionsService transactionService , TransactionDbContext dbContext)
        {
            _transactionService = transactionService;
            _dbContext = dbContext;
        }
        //[Transactional]
        [HttpPost("UpdateTransation")]
        public async Task<IActionResult> UpdateTransactions([FromBody] UpdateTransactionsCommand command)
        {

            try
            {
                // Check if the transaction update request is a duplicate
                if (_transactionService.IsDuplicateTransactionRequest(command))
                {
                    // If it's a duplicate, return a conflict response
                    return Conflict("Duplicate transaction update request");
                }

                // Update the transactions
                _transactionService.UpdateTransactions(command);

                // Return a success response
                return Ok();
            }
            catch (Exception ex)
            {
                // Log the exception
                // You can use your chosen logging mechanism here

                // Return an error response
                return StatusCode(500, "An error occurred while updating transactions");
            }
        }

        [HttpGet("{clientId}")]
        public async Task<IActionResult> GetTransactionsByClientId(int clientId)
        {
            try
            {
                // Retrieve the transactions for the given client ID
                var transactions = _dbContext.Transactions.Where(x => x.ClientId == clientId).ToList();

                // Return the transactions[k[i
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                // Log the exception
                // You can use your chosen logging mechanism here

                // Return an error response
                return StatusCode(500, "An error occurred while retrieving transactions");
            }
        }
    }
}
