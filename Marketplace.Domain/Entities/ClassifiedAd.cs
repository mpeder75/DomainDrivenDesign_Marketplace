using System.ComponentModel;
using Marketplace.Domain.Events;
using Marketplace.Domain.Exceptions;
using Marketplace.Domain.ValueObjects;
using Marketplace.Framework.Events;

namespace Marketplace.Domain.Entities;

// nedarver fra Events
public class ClassifiedAd : Entity
{
    public ClassifiedAdId Id { get; init; }
    public UserId OwnerId { get; init; }
    public ClassifiedAdTitle Title { get; private set; }
    public ClassifiedAdText Text { get;  private set; }
    public Price Price { get;  private set; }
    public ClassifiedAdState State { get;  private set; }
    public UserId ApprovedBy { get;  private set; }

    public ClassifiedAd(ClassifiedAdId id, UserId ownerId)
    {
        Id = id;
        OwnerId = ownerId;
        State = ClassifiedAdState.Inactive;
        EnsureValidState();

        Raise(new Events.Events.ClassifiedAdCreated(
        {
            Id = id,
            OwnerId = ownerId
        });
    }

    public void SetTitle(ClassifiedAdTitle title)
    {
        Title = title;
        EnsureValidState(); 
        Raise(new Events.Events.ClassifiedAdTitleChanged
        {
            Id = Id,
            Title = title
        });
    }

    public void UpdateText(ClassifiedAdText text)
    {
        Text = text;
        EnsureValidState();
        Raise(new Events.Events.ClassifiedAdTextUpdated(
        {
            Id = Id,
            AdText = text
        });
    }

    public void UpdatePrice(Price price)
    {
        Price = price;
        EnsureValidState();
        Raise(new Events.Events.ClassifiedAdPriceUpdated
        {
            Id = Id,
            Price = Price.Amount,
            CurrencyCode = Price.Currency.CurrencyCode
        });
    }

    public void RequestToPublish()
    {
        State = ClassifiedAdState.PendingReview;
        EnsureValidState();
        Raise(new Events.Events.ClassidiedAdSentForReview {Id = id});
    }

    
    protected void EnsureValidState()
    {
        var valid = Id != null && OwnerId != null &&
                    (State switch
                    {
                        ClassifiedAdState.PendingReview =>
                            Title != null && Text != null && Price?.Amount > 0,
                        ClassifiedAdState.Active =>
                            Title != null && Text != null && Price?.Amount > 0 && ApprovedBy != null,
                        _ => true
                    });

        if (!valid) 
        {
            throw new InvalidEntityStateException(this, $"Post-checks failed in state {State}");
        }
    }

    public enum ClassifiedAdState
    {
        PendingReview,
        Active,
        Inactive,
        MarkedAsSold
    }
}