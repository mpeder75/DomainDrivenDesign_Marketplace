namespace Marketplace.ClassifiedAd
{
    public static class Contracts
    {
        public static class V1
        {
            public record Create
            {
                public Guid Id { get; init; }
                public Guid OwnerId { get; init; }
            }

            public record SetTitle
            {
                public Guid Id { get; init; }
                public string Title { get; init; }
            }

            public record UpdateText
            {
                public Guid Id { get; init; }
                public string Text { get; init; }
            }

            public record UpdatePrice
            {
                public Guid Id { get; init; }
                public decimal Price { get; init; }
                public string Currency { get; init; }
            }

            public record RequestToPublish
            {
                public Guid Id { get; init; }
            }

            public record Publish
            {
                public Guid Id { get; init; }
                public Guid ApprovedBy { get; init; }
            }
        }
    }
}
