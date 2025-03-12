using Marketplace.Domain.ValueObjects;

namespace Marketplace.Domain.Services;

public interface ICurrencyLookup
{
    CurrencyDetails FindCurrency(string currencyCode);
}