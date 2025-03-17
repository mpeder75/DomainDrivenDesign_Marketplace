using Marketplace.Framework.Common;
using Marketplace.Framework.Events;

namespace Marketplace.Framework;

public abstract class DomainEntity<TId> : IInternalEventHandler where TId : Value<TId>
{
    private readonly Action<object> _applier;

    public TId Id { get; protected set; }

    protected DomainEntity(Action<object> applier)
    {
        _applier = applier;
    }

    void IInternalEventHandler.Handle(object @event)
    {
        When(@event);
    }

    protected abstract void When(object @event);

    protected void Apply(object @event)
    {
        When(@event);
        _applier(@event);
    }
}

