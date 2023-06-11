using System;
using System.Transactions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Xend.Transaction.Api.Handlers
{


    public class Transactional : Attribute, IActionFilter
    {
        private TransactionScope _transactionScope;

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Begin the transaction
            _transactionScope = new TransactionScope();
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception == null)
            {
                // Complete the transaction if no exception occurred
                _transactionScope.Complete();
            }
            else
            {
                // Rollback the transaction if an exception occurred
                _transactionScope.Dispose();
            }

            // Dispose the transaction scope
            _transactionScope.Dispose();
        }
    }

}
