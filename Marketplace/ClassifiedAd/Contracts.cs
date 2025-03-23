namespace Marketplace.ClassifiedAd
{
    public static class Contracts
    {
        public static class V1
        {
            public class Create
            {
                public Guid Id { get; init; }
                public Guid OwnerId { get; init; }
            }

            public class SetTitle
            {
                public Guid Id { get; init; }
                public string Title { get; init; }
            }

            public class UpdateText
            {
                public Guid Id { get; init; }
                public string Text { get; init; }
            }

            public class UpdatePrice
            {
                public Guid Id { get; init; }
                public decimal Price { get; init; }
                public string Currency { get; init; }
            }

            public class RequestToPublish
            {
                public Guid Id { get; init; }
            }

            public class Publish
            {
                public Guid Id { get; init; }
                public Guid ApprovedBy { get; init; }
            }
        }
    }
}
