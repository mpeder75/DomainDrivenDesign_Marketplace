using Marketplace.Framework.Common;

namespace Marketplace.Domain.ValueObjects;

public class ClassifiedAdId : Value<ClassifiedAdId>
{
    private readonly Guid _value;

    public ClassifiedAdId(Guid value)
    {
        if (value == default)
        {
            throw new ArgumentNullException( nameof(value),"Classified Ad id cannot be empty");
        }

        _value = value;
    }

    // Factory method, der konverterer en string til en ClassifiedAdId
    public static implicit operator Guid(ClassifiedAdId self) => self._value;

    public static implicit operator ClassifiedAdId(string value) 
        => new ClassifiedAdId(Guid.Parse(value));
}