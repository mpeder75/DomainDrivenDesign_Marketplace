using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Marketplace.Framework.DomainService;

public abstract class Entity
{
    private readonly List<object> _events;

    protected Entity() => _events = new List<object>();

    // Metode til at håndtere state changes via events
    protected void Apply(object @event)
    {
        // Opdaterer entitetstilstanden baseret på eventet
        When(@event);         
        EnsureValidState();
        
        // Tilføjer eventet til listen af ændringer
        _events.Add(@event);  
    }
   
    // Abstrakt metode - hver entitet skal definere sin egen event-håndtering
    protected abstract void When(object @event);

    public IEnumerable<object> GetChanges() => _events.AsEnumerable();

    public void ClearChanges() => _events.Clear();
    
    // Metode der sikrer at entiteten er i en gyldig tilstand
    protected abstract void EnsureValidState();
}

