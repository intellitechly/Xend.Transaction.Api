namespace Xend.Transaction.Api.Contracts
{
    public interface IEventHandler<TEvent>
    {
        void Handle(TEvent @event);
    }
}
