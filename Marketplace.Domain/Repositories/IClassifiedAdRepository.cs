using Marketplace.Domain.Entities;
using Marketplace.Domain.ValueObjects;

namespace Marketplace.Domain.Repositories;

public interface IClassifiedAdRepository
{
    Task<bool> Exists(ClassifiedAdId id);

    Task<ClassifiedAd> Load(ClassifiedAdId id);

    Task Save(ClassifiedAd entity);
}