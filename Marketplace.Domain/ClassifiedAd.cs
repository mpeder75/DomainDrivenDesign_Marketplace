namespace Marketplace.Domain;

public class ClassifiedAd
{
    // Guid, kan kun sættes i constructor
    public Guid Id { get; }

    public ClassifiedAd(Guid id)
    {
        if (id == default)
        {
            throw new ArgumentException("Id must be specific", nameof(id));
        }

        Id = id;
    }

    private Guid _ownerId;
    private string _title;
    private string _text;
    private decimal _price;
}