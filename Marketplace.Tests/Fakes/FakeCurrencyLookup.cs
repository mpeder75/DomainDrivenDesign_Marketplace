﻿using Marketplace.Domain.Services;
using Marketplace.Domain.ValueObjects;

namespace Marketplace.Tests.Fakes;

public class FakeCurrencyLookup : ICurrencyLookup
{
    private static readonly IEnumerable<CurrencyDetails> _currencies = new[]
    {
        new CurrencyDetails { CurrencyCode = "EUR", DecimalPlaces = 2, InUse = true },
        new CurrencyDetails { CurrencyCode = "USD", DecimalPlaces = 2, InUse = true },
        new CurrencyDetails { CurrencyCode = "JPY", DecimalPlaces = 0, InUse = true },
        new CurrencyDetails { CurrencyCode = "DEM", DecimalPlaces = 2, InUse = false }
    };

    public CurrencyDetails FindCurrency(string currencyCode)
    {
        var currency = _currencies.FirstOrDefault(x => x.CurrencyCode == currencyCode);
        return currency ?? CurrencyDetails.None;
    }
}