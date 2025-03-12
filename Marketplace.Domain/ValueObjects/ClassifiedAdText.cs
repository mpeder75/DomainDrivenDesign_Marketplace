namespace Marketplace.Domain.ValueObjects;

public record ClassifiedAdText
{
    public string Value { get; }

    // Constructor, der tager imod en string og sætter Value til den modtagne string
    internal ClassifiedAdText(string text) => Value = text;

    // Factory method, der konverterer en string til en ClassifiedAdText
    public static ClassifiedAdText FromString(string text) =>
        new ClassifiedAdText(text);

    // Factory method, der konverterer en html-streng til en ClassifiedAdText
    public static implicit operator string(ClassifiedAdText text) => text.Value;
}