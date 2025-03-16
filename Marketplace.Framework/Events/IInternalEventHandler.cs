namespace Marketplace.Framework.Events;

public interface IInternalEventHandler
{
    void Handle(object @event);
}