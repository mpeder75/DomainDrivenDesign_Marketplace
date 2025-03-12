namespace Marketplace.Domain.ValueObjects;

public record ClassifiedAdText
{
    public string Value { get; }

    public ClassifiedAdText(string text)
    {
        Value = text;
    }

    public static ClassifiedAdText FromString(string text)
    {
        return new ClassifiedAdText(text);
    }

    public static implicit operator string(ClassifiedAdText text)
    {
        return text.Value;
    }

    public static implicit operator ClassifiedAdText(string text)
    {
        return new ClassifiedAdText(text);
    }
}