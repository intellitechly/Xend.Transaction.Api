namespace Xend.Transaction.Api.Contracts
{
    public interface IEventBus
    {
        void Publish<TEvent>(TEvent @event);
    }
}
