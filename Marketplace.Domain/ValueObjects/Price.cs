using Marketplace.Domain.Services;

namespace Marketplace.Domain.ValueObjects;

public record Price : Money
{
    public Price(decimal amount, string currencyCode, ICurrencyLookup currencyLookup)
        : base(amount, currencyCode, currencyLookup)
    {
        if (amount < 0)
        {
            throw new ArgumentException("Price cannot be negative", nameof(amount));
        }
    }

    public static new Price FromDecimal(decimal amount, string currencyCode, ICurrencyLookup currencyLookup)
    {
        return new Price(amount, currencyCode, currencyLookup);
    }
}