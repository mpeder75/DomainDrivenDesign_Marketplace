using Marketplace.Domain.ValueObjects;
using Marketplace.Framework.Common;

namespace Marketplace.Domain.Services;

public interface ICurrencyLookup
{
    Currency FindCurrency(string currencyCode);
}

public class Currency : Value<Currency>
{
    public string CurrencyCode { get; set; }
    public bool InUse { get; set; }
    public int DecimalPlaces { get; set; }

    public static Currency None = new Currency {InUse = false};
}
