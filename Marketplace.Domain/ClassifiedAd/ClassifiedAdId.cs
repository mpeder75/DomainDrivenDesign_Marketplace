using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAd;

public class ClassifiedAdId : Value<ClassifiedAdId>
{
    public Guid Value;
    
    public ClassifiedAdId(Guid value)
    {
        if (value == default)
        {
            throw new ArgumentNullException( nameof(value),"Classified Ad id cannot be empty");
        }

        Value = value;
    }

    // Factory method, der konverterer en string til en ClassifiedAdId
    public static implicit operator Guid(ClassifiedAdId self) => self.Value;

    public static implicit operator ClassifiedAdId(string value) 
        => new ClassifiedAdId(Guid.Parse(value));

    public override string ToString() => Value.ToString();
}