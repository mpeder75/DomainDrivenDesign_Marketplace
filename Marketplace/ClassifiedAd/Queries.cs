using Marketplace.Projections;
using Raven.Client.Documents.Session;
using static Marketplace.ClassifiedAd.QueryModels;

namespace Marketplace.ClassifiedAd
{
    public static class Queries
    {
        public static Task<ReadModels.ClassifiedAdDetails> Query(
            this IAsyncDocumentSession session,
            GetPublicClassifiedAd query
        ) =>
            session.LoadAsync<ReadModels.ClassifiedAdDetails>(
                query.ClassifiedAdId.ToString()
            );
    }
}