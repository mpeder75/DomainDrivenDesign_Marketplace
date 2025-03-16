using Marketplace.Domain.Entities;
using Marketplace.Domain.ValueObjects;

namespace Marketplace.Domain.Repositories;

public interface IClassifiedAdRepository
{
    Task Add(ClassifiedAd entity);

    Task<ClassifiedAd> Load(ClassifiedAdId id);

    Task<bool> Exists(ClassifiedAdId id);
}

