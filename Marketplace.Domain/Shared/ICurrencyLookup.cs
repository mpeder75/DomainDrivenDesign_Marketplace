﻿using Marketplace.Framework;

namespace Marketplace.Domain.Shared;

public interface ICurrencyLookup
{
    Currency FindCurrency(string currencyCode);
}

public class Currency : Value<Currency>
{
    public static Currency None = new() { InUse = false };
    public string CurrencyCode { get; set; }
    public bool InUse { get; set; }
    public int DecimalPlaces { get; set; }
}