namespace Marketplace.Domain.ValueObjects
{
    public record CurrencyDetails
    {
        public string CurrencyCode { get; init; }
        public bool InUse { get; init; }
        public int DecimalPlaces { get; init; }

        // protected constructor for serialization
        protected CurrencyDetails() { }

        public static CurrencyDetails None = new CurrencyDetails
        {
            InUse = false
        };
    }
}