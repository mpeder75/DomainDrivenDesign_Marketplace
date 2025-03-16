using Marketplace.Domain.Exceptions;
using Marketplace.Domain.Services;
using Marketplace.Domain.ValueObjects;
using Marketplace.Framework;

namespace Marketplace.Domain.Entities;

public class ClassifiedAd : AggregateRoot<ClassifiedAdId>
{
    public ClassifiedAdId Id { get; private set; }
    public UserId OwnerId { get; private set; }
    public ClassifiedAdTitle Title { get; private set; }
    public ClassifiedAdText Text { get; private set; }
    public Price Price { get; private set; }
    public ClassifiedAdState State { get; private set; }
    public UserId ApprovedBy { get; private set; }
    // Liste af billeder
    public List<Picture> Pictures { get; private set; }


    public ClassifiedAd(ClassifiedAdId id, UserId ownerId)
    {
        // Initialiser listen af billeder
        Pictures = new List<Picture>();

        Apply(new Events.Events.ClassifiedAdCreated
        {
            Id = id,
            OwnerId = ownerId
        });
    }
        
    public void SetTitle(ClassifiedAdTitle title) => Apply(new Events.Events.ClassifiedAdTitleChanged
    {
        Id = Id,
        Title = title
    });

    public void UpdateText(ClassifiedAdText text) => Apply(new Events.Events.ClassifiedAdTextUpdated
    {
        Id = Id,
        AdText = text
    });

    public void UpdatePrice(Price price) => Apply(new Events.Events.ClassifiedAdPriceUpdated
    {
        Id = Id,
        Price = price.Amount,
        CurrencyCode = price.Currency.CurrencyCode
    });

    public void RequestToPublish() => Apply(new Events.Events.ClassifiedAdSentForReview
    {
        Id = Id
    });

    public void ResizePicture(PictureId pictureId, PictureSize newSize)
    {
        var picture = FindPicture(pictureId);
        if (picture == null)
            throw new InvalidOperationException(
                "Cannot resize a picture that I don't have"
            );

        picture.Resize(newSize);
    }

    protected override void When(object @event)
    {
        Picture picture;

        switch (@event)
        {
            case Events.Events.ClassifiedAdCreated e:
                Id = new ClassifiedAdId(e.Id);
                OwnerId = new UserId(e.OwnerId);
                State = ClassifiedAdState.Inactive;
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
            case Events.Events.ClassifiedAdSentForReview _:
                State = ClassifiedAdState.PendingReview;
                break;
            // Tilføjelse af billede til classified ad
            case Events.Events.PictureAddedToAClassifiedAd e:
                var newPicture = new Picture(Apply);
                ApplyToEntity(newPicture, e); // Brug newPicture i stedet for picture
                Pictures.Add(newPicture);
                break;
            // Event til når et billede er blevet resized
            case Events.Events.ClassifiedAdPictureResized e:
                picture = FindPicture(new PictureId(e.PictureId));
                ApplyToEntity(picture, @event);
                break;
        }
    }

    private Picture FindPicture(PictureId id)
        => Pictures.FirstOrDefault(x => x.Id == id);

    private Picture FirstPicture
        => Pictures.OrderBy(x => x.Order).FirstOrDefault();

    public void AddPicture(Uri pictureUri, PictureSize size)
    {
        Apply(
            new Events.Events.PictureAddedToAClassifiedAd
            {
                PictureId = new Guid(),
                ClassifiedAdId = Id,
                Url = pictureUri.ToString(),
                Height = size.Height,
                Width = size.Width,
                Order = NewPictureOrder()
            }
        );

        int NewPictureOrder()
            => Pictures.Any()
                ? Pictures.Max(x => x.Order) + 1
                : 0;
    }

    protected override void EnsureValidState()
    {
        var valid =
            Id != null &&
            OwnerId != null &&
            (State switch
            {
                ClassifiedAdState.PendingReview =>
                    Title != null
                    && Text != null
                    && Price?.Amount > 0
                    && FirstPicture.HasCorrectSize(),
                ClassifiedAdState.Active =>
                    Title != null
                    && Text != null
                    && Price?.Amount > 0
                    && FirstPicture.HasCorrectSize()
                    && ApprovedBy != null,
                _ => true
            });

        if (!valid)
            throw new InvalidEntityStateException(this, $"Post-checks failed in state {State}");
    }

    public enum ClassifiedAdState
    {
        PendingReview,
        Active,
        Inactive,
        MarkedAsSold
    }
}