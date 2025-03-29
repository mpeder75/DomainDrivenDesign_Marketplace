namespace Marketplace.ClassifiedAd;

public static class Contracts
    {
        public static class V1
        {
            public class Create
            {
                public Guid Id { get; set; }
                public Guid OwnerId { get; set; }
            }

            public class SetTitle
            {
                public Guid Id { get; set; }
                public string Title { get; set; }
            }

            public class UpdateText
            {
                public Guid Id { get; set; }
                public string Text { get; set; }
            }

            public class UpdatePrice
            {
                public Guid Id { get; set; }
                public decimal Price { get; set; }
                public string Currency { get; set; }
            }

            public class RequestToPublish
            {
                public Guid Id { get; set; }
            }

            public class Publish
            {
                public Guid Id { get; set; }
                public Guid ApprovedBy { get; set; }
            }

            public class ClassifiedAdPublished
            {
                public Guid Id { get; set; }
                public string SellersPhotoUrl { get; set; }
                // You might need additional properties such as OwnerId and ApprovedBy
                // based on other uses of this class in your application
            }
        }
    }

