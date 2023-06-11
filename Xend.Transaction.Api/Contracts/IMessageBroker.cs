namespace Xend.Transaction.Api.Contracts
{
    public interface IMessageBroker
    {
        void Publish<TEvent>(TEvent @event);
    }
}
