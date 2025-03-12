using Marketplace.Domain.Services;

namespace Marketplace.Domain.ValueObjects;

public record Price : Money
{
    // Constructor, der tager imod et decimal, en string og en ICurrencyLookup og kalder base-klassen
    private Price(decimal amount, string currencyCode, ICurrencyLookup currencyLookup)
        : base(amount, currencyCode, currencyLookup)
    {
        if (amount < 0)
            throw new ArgumentException(
                "Price cannot be negative",
                nameof(amount));
    }

    // Constructor, der tager imod et decimal og en string og kalder base-klassen
    internal Price(decimal amount, string currencyCode)
        : base(amount, new CurrencyDetails { CurrencyCode = currencyCode }) { }

    public new static Price FromDecimal(decimal amount, string currency,
        ICurrencyLookup currencyLookup) =>
        new Price(amount, currency, currencyLookup);
}
