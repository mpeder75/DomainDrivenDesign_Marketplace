using System.Runtime.CompilerServices;

namespace Marketplace.Framework.Events;

public abstract class Entity
{
    // Liste til at holde styr på de events, der er blevet rejst af entiteten
    private readonly List<object> _events;

    // Constructor, der initialiserer listen
    protected Entity() => _events = new List<object>();

    // Metode til at rejse et event og tilføje det til listen over events
    protected void Raise(object @event) => _events.Add(@event);

    // Metode til at hente en kopi af listen over events
    public IEnumerable<object> GetChanges() => _events.AsEnumerable();

    public void ClearChanges() => _events.Clear();
}

