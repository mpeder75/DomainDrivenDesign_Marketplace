namespace Marketplace.Domain.ValueObjects;

public record UserId
{
    private Guid Value { get; set; }

    public UserId(Guid value)
    {
        if (value == default)
        {
            throw new ArgumentNullException("User id cannot be empty", nameof(value));
        }

        Value = value;
    }

    public static implicit operator Guid(UserId self) => self.Value;

}