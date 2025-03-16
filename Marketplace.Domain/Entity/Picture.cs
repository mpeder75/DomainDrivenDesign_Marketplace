using Marketplace.Domain.ValueObjects;
using Marketplace.Framework;
using Marketplace.Framework.Common;
using Marketplace.Framework.Events;

namespace Marketplace.Domain.Entities;

public class Picture : DomainEntity<PictureId>
{
    internal ClassifiedAdId ParentId { get; private set; }
    internal PictureSize Size { get; private set; }
    internal Uri Location { get; private set; }
    internal int Order { get; private set; }

    protected override void When(object @event)
    {
        switch (@event)
        {
            // Event til når et billede er blevet tilføjet til en classified ad
            case Events.Events.PictureAddedToAClassifiedAd e:
                ParentId = new ClassifiedAdId(e.ClassifiedAdId);
                Id = new PictureId(e.PictureId);
                Location = new Uri(e.Url);
                Size = new PictureSize { Height = e.Height, Width = e.Width };
                Order = e.Order;
                break;
            // Event til når et billede er blevet resized
            case Events.Events.ClassifiedAdPictureResized e:
                Size = new PictureSize { Height = e.Height, Width = e.Width };
                break;
        }
    }

    public void Resize(PictureSize newSize)
        => Apply(new Events.Events.ClassifiedAdPictureResized
        {
            PictureId = Id.Value,
            Height = newSize.Height,
            Width = newSize.Width
        });

    public Picture(Action<object> applier) : base(applier) { }
}

public class PictureId : Value<PictureId>
{
    public PictureId(Guid value) => Value = value;

    public Guid Value { get; }
}