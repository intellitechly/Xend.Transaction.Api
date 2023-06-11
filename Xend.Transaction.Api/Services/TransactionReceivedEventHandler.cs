using Xend.Transaction.Api.Contracts;
using Xend.Transaction.Api.Data.Sequence;

namespace Xend.Transaction.Api.Services
{
    public class TransactionReceivedEventHandler : IEventHandler<TransactionReceivedEvent>
    {
        private readonly ITransactionService _transactionRepository;
        private readonly IEventBus _eventBus;

        public TransactionReceivedEventHandler(ITransactionService transactionRepository, IEventBus eventBus)
        {
            _transactionRepository = transactionRepository;
            _eventBus = eventBus;
        }

        public void Handle(TransactionReceivedEvent @event)
        {
            // Check if the transaction already exists
            if (_transactionRepository.DoesTransactionExist(@event.Transaction.TransactionId))
            {
                // If the transaction already exists, log and return
                //Console.WriteLine($"Duplicate transaction received. TransactionId: {@event.TransactionId}");
                return;
            }

            // Process the new transaction
           // perform any other necessary actions

            // Publish the transaction received event to the event bus
            _eventBus.Publish(new TransactionReceivedEvent
            {

                ClientId = @event.ClientId,
                Transaction = new Data.Entities.WalletTransaction
                {
                    TransactionId = @event.Transaction.TransactionId,
                    Timestamp = @event.Transaction.Timestamp
                    

                }
              
            });
        }
    }

}
