using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Queries;
using Raven.Client.Documents.Session;
using static Marketplace.ClassifiedAd.ReadModels;
using static Marketplace.Domain.ClassifiedAd.ClassifiedAd;

namespace Marketplace.ClassifiedAd
{
    public static class Queries
    {
        // GetAll publicerede annoncer for en ejer
        public static Task<List<PublicClassifiedAdListItem>> Query(
            this IAsyncDocumentSession session,
            QueryModels.GetPublishedClassifiedAds query
        ) =>
            session.Query<Domain.ClassifiedAd.ClassifiedAd>() // Forespørgsel på alle aktive annoncer

                .Where(x => x.State == ClassifiedAdState.Active)
                .Select(
                    x =>
                        new PublicClassifiedAdListItem
                        {
                            ClassifiedAdId = x.Id.Value,
                            Price = x.Price.Amount,
                            Title = x.Title.Value,
                            CurrencyCode = x.Price.Currency.CurrencyCode
                        }
                )
                .PagedList(query.Page, query.PageSize);

        // Query for classified ads owned by a user
        public static Task<List<PublicClassifiedAdListItem>> Query(
            this IAsyncDocumentSession session,
            QueryModels.GetOwnersClassifiedAd query
        )
            =>
                session.Query<Domain.ClassifiedAd.ClassifiedAd>()
                    .Where(x => x.OwnerId.Value == query.OwnerId)
                    .Select(
                        x =>
                            new PublicClassifiedAdListItem
                            {
                                ClassifiedAdId = x.Id.Value,
                                Price = x.Price.Amount,
                                Title = x.Title.Value,
                                CurrencyCode = x.Price.Currency.CurrencyCode
                            }
                    )
                    .PagedList(query.Page, query.PageSize);

        // Query for en specifik classified ad
        public static Task<ReadModels.ClassifiedAdDetails> Query(
            this IAsyncDocumentSession session,
            QueryModels.GetPublicClassifiedAd query)
            => (from ad in session.Query<Domain.ClassifiedAd.ClassifiedAd>()
                where ad.Id.Value == query.ClassifiedAdId
                let user = RavenQuery
                    .Load<Domain.UserProfile.UserProfile>("UserProfile/" + ad.OwnerId.Value)
                select new ReadModels.ClassifiedAdDetails
                {
                    ClassifiedAdId = ad.Id.Value,
                    Title = ad.Title.Value,
                    Description = ad.Text.Value,
                    Price = ad.Price.Amount,
                    CurrencyCode = ad.Price.Currency.CurrencyCode,
                    SellersDisplayName = user.DisplayName.Value
                }).SingleAsync();


        // Helper method for pagination
        private static Task<List<T>> PagedList<T>(
            this IRavenQueryable<T> query, int page, int pageSize) =>
            query
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync();
    }
}