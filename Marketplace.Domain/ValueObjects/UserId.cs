namespace Marketplace.Domain.ValueObjects;

public record UserId
{
    private readonly Guid _value;

    public UserId(Guid value)
    {
        if (value == default)
        {
            throw new ArgumentNullException("User id cannot be empty", nameof(value));
        }

        _value = value;
    }

    public static implicit operator Guid(UserId self) => self._value;

}