using Marketplace.Framework;

namespace Marketplace.Domain.UserProfile;

public class FullName : Value<FullName>
{
    
    public string Value { get; private set; }

    // Constructor der sætter værdien af Value
    internal FullName(string value) => Value = value;

    // Satisfy the serialization requirements
    protected FullName() { }

    // Factory metode der returnerer en ny instans af FullName
    public static FullName FromString(string fullName)
    {
        if (string.IsNullOrEmpty((fullName)))
            throw new ArgumentNullException(nameof(fullName));
            
        return new FullName(fullName);
    }

    // Implicit operator der konverterer en instans af FullName til en string
    public static implicit operator string(FullName fullName)
        => fullName.Value;
        
}