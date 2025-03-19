using Marketplace.Framework.Events;

namespace Marketplace.Framework;

public abstract class AggregateRoot<TId> : IInternalEventHandler where TId : Value<TId>
{
    public TId Id { get; protected set; }
    protected abstract void When(object @event);
    private readonly List<object> _changes;

    protected AggregateRoot() => _changes = new List<object>();

    protected void Apply(object @event)
    {
        When(@event);
        EnsureValidState();
        _changes.Add(@event);
    }

    public IEnumerable<object> GetChanges() => _changes.AsEnumerable();

    public void ClearChanges() => _changes.Clear();

    protected abstract void EnsureValidState();

    // IInternalEventHandler
    protected void ApplyToEntity(IInternalEventHandler entity, object @event) => entity.Handle(@event);

    // IInternalEventHandler
    void IInternalEventHandler.Handle(object @event) => When(@event);
}



