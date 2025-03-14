using Marketplace.Domain.Exceptions;
using Marketplace.Domain.ValueObjects;
using Marketplace.Framework.DomainService;
using Marketplace.Framework.DomainService;

namespace Marketplace.Domain.Entities;

// Nedarver fra Entity for at håndtere events og event sourcing
public class ClassifiedAd : Entity
{
    public ClassifiedAdId Id { get; private set; }
    public UserId OwnerId { get; private set; }
    public ClassifiedAdTitle Title { get; private set; }
    public ClassifiedAdText Text { get; private set; }
    public Price Price { get; private set; }
    public ClassifiedAdState State { get; private set; }
    public UserId ApprovedBy { get; private set; }

    // Constructor, der opretter en ny annonce og anvender et ClassifiedAdCreated event
    public ClassifiedAd(ClassifiedAdId id, UserId ownerId) =>
        Apply(new Events.Events.ClassifiedAdCreated
        {
            Id = id,
            OwnerId = ownerId
        });

    // Metode til at sætte titlen på annoncen og anvende et ClassifiedAdTitleChanged event
    public void SetTitle(ClassifiedAdTitle title) => Apply(new Events.Events.ClassifiedAdTitleChanged
    {
        Id = Id,
        Title = title
    });

    // Metode til at opdatere teksten på annoncen og anvende et ClassifiedAdTextUpdated event
    public void UpdateText(ClassifiedAdText text) => Apply(new Events.Events.ClassifiedAdTextUpdated
    {
        Id = Id,
        AdText = text
    });
    
    // Metode til at opdatere prisen på annoncen og anvende et ClassifiedAdPriceUpdated event
    public void UpdatePrice(Price price) => Apply(new Events.Events.ClassifiedAdPriceUpdated
    {
        Id = Id,
        Price = price.Amount,
        CurrencyCode = price.Currency.CurrencyCode
    });

    // Metode til at anmode om at få annoncen publiceret og anvende et ClassifiedAdSentForReview event
    public void RequestToPublish() => Apply(new Events.Events.ClassifiedAdSentForReview
    {
        Id = Id
    });

    // Metode til at håndtere events og opdatere entitetens tilstand baseret på eventet
    protected override void When(object @event)
    {
        switch (@event)
        {
            case Events.Events.ClassifiedAdCreated e:
                Id = new ClassifiedAdId(e.Id);
                OwnerId = new UserId(e.OwnerId);
                State = ClassifiedAdState.PendingReview;
                break;
            case Events.Events.ClassifiedAdTitleChanged e:
                Title = new ClassifiedAdTitle(e.Title);
                break;
            case Events.Events.ClassifiedAdTextUpdated e:
                Text = new ClassifiedAdText(e.AdText);
                break;  
            case Events.Events.ClassifiedAdPriceUpdated e:
                Price = new Price(e.Price, e.CurrencyCode);
                break;
            case Events.Events.ClassifiedAdSentForReview e:
                State = ClassifiedAdState.PendingReview;
                break;
        }
    }

    // Metode til at sikre, at entitetens tilstand altid er gyldig efter et event er anvendt
    protected override void EnsureValidState()
    {
        var valid = 
            Id != null && OwnerId != null &&
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