namespace Marketplace.Domain.ClassifiedAd;

public interface IClassifiedAdRepository
{
    Task Add(ClassifiedAd entity);

    Task<ClassifiedAd> Load(ClassifiedAdId id);

    Task<bool> Exists(ClassifiedAdId id);
}

