﻿using Marketplace.Domain.ClassifiedAd;
using Raven.Client.Documents.Identity;
using Raven.Client.Documents.Session;

namespace Marketplace.ClassifiedAd
{ public class ClassifiedAdRepository : IClassifiedAdRepository, IDisposable
    {
        private readonly IAsyncDocumentSession _session;

        public ClassifiedAdRepository(IAsyncDocumentSession session) 
            => _session = session;

        public Task Add(Domain.ClassifiedAd.ClassifiedAd entity) 
            => _session.StoreAsync(entity, EntityId(entity.Id));

        public Task<bool> Exists(ClassifiedAdId id) 
            => _session.Advanced.ExistsAsync(EntityId(id));

        public Task<Domain.ClassifiedAd.ClassifiedAd> Load(ClassifiedAdId id)
            => _session.LoadAsync<Domain.ClassifiedAd.ClassifiedAd>(EntityId(id));

        public void Dispose() => _session.Dispose();
        
        private static string EntityId(ClassifiedAdId id)
            => $"ClassifiedAd/{id.ToString()}";
    }
}
