﻿using Marketplace.Domain.Shared;
using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAd;

public class ClassifiedAd : AggregateRoot<ClassifiedAdId>
    {
        // Properties to handle the persistence
        private string DbId
        {
            get => $"ClassifiedAd/{Id.Value}";
            set {}
        }
        
        // Aggregate state properties
        public UserId OwnerId { get; private set; }
        public ClassifiedAdTitle Title { get; private set; }
        public ClassifiedAdText Text { get; private set; }
        public Price Price { get; private set; }
        public ClassifiedAdState State { get; private set; }
        public UserId ApprovedBy { get; private set; }
        public List<Picture> Pictures { get; }

        public ClassifiedAd(ClassifiedAdId id, UserId ownerId)
        {
            Pictures = new List<Picture>();
            Apply(new Events.ClassifiedAdCreated
            {
                Id = id,
                OwnerId = ownerId
            });
        }

        public void SetTitle(ClassifiedAdTitle title) =>
            Apply(new Events.ClassifiedAdTitleChanged
            {
                Id = Id,
                Title = title
            });

        public void UpdateText(ClassifiedAdText text) =>
            Apply(new Events.ClassifiedAdTextUpdated
            {
                Id = Id,
                AdText = text
            });

        public void UpdatePrice(Price price) =>
            Apply(new Events.ClassifiedAdPriceUpdated
            {
                Id = Id,
                Price = price.Amount,
                CurrencyCode = price.Currency.CurrencyCode
            });

        public void RequestToPublish() =>
            Apply(new Events.ClassidiedAdSentForReview {Id = Id});
        
        public void Publish(UserId userId) =>
            Apply(new Events.ClassifiedAdPublished {Id = Id, ApprovedBy = userId});
        
        public void AddPicture(Uri pictureUri, PictureSize size)
        {
            Apply(new Events.PictureAddedToAClassifiedAd
            {
                PictureId = new Guid(),
                ClassifiedAdId = Id,
                Url = pictureUri.ToString(),
                Height = size.Height,
                Width = size.Width,
                Order = NewPictureOrder()
            });
            
            int NewPictureOrder() => Pictures.Any() ? Pictures.Max(x => x.Order) + 1 : 0;
        }

        public void ResizePicture(PictureId pictureId, PictureSize newSize)
        {
            var picture = FindPicture(pictureId);
            if (picture == null)
                throw new InvalidOperationException("Cannot resize a picture that I don't have");
            
            picture.Resize(newSize);
        }

        protected override void When(object @event)
        {
            Picture picture;
            
            switch (@event)
            {
                case Events.ClassifiedAdCreated e:
                    Id = new ClassifiedAdId(e.Id);
                    OwnerId = new UserId(e.OwnerId);
                    State = ClassifiedAdState.Inactive;
                    break;
                case Events.ClassifiedAdTitleChanged e:
                    Title = new ClassifiedAdTitle(e.Title);
                    break;
                case Events.ClassifiedAdTextUpdated e:
                    Text = new ClassifiedAdText(e.AdText);
                    break;
                case Events.ClassifiedAdPriceUpdated e:
                    Price = new Price(e.Price, e.CurrencyCode);
                    break;
                case Events.ClassidiedAdSentForReview _:
                    State = ClassifiedAdState.PendingReview;
                    break;
                case Events.ClassifiedAdPublished e:
                    ApprovedBy = new UserId(e.ApprovedBy);
                    State = ClassifiedAdState.Active;
                    break;
                
                // picture
                case Events.PictureAddedToAClassifiedAd e:
                    picture = new Picture(Apply);
                    ApplyToEntity(picture, e);
                    Pictures.Add(picture);
                    break;
                case Events.ClassifiedAdPictureResized e:
                    picture = FindPicture(new PictureId(e.PictureId));
                    ApplyToEntity(picture, @event);
                    break;
            }
        }

        private Picture FindPicture(PictureId id)
            => Pictures.FirstOrDefault(x => x.Id == id);

        private Picture FirstPicture => 
            Pictures.OrderBy(x => x.Order)
                .FirstOrDefault();

        protected override void EnsureValidState()
        {
            var valid = Id != null && OwnerId != null &&
            (State switch
            {
                ClassifiedAdState.PendingReview =>
                    Title != null
                    && Text != null
                    && Price?.Amount > 0,
                ClassifiedAdState.Active =>
                    Title != null
                    && Text != null
                    && Price?.Amount > 0
                    && ApprovedBy != null,
                _ => true
            });

            if (!valid)
                throw new DomainExceptions.InvalidEntityState(this, 
                    $"Post-checks failed in state {State}");
        }

        public enum ClassifiedAdState
        {
            PendingReview,
            Active,
            Inactive,
            MarkedAsSold
        }
    }