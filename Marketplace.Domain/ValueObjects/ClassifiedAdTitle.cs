using System.Text.RegularExpressions;

namespace Marketplace.Domain.ValueObjects;

public record ClassifiedAdTitle
{
    public string Value { get; }

    // Constructor, der tager imod en string og sætter Value til den modtagne string
    internal ClassifiedAdTitle(string value)
    {
        Value = value;
    }

    // Factory method, der konverterer en string til en ClassifiedAdTitle
    public static ClassifiedAdTitle FromString(string title)
    {
        CheckValidity(title);
        return new ClassifiedAdTitle(title);
    }

    // Factory method, der konverterer en html-streng til en ClassifiedAdTitle
    public static ClassifiedAdTitle FromHtml(string htmlTitle)
    {
        var supportedTagsReplaced = htmlTitle
            .Replace("<i>", "*")
            .Replace("</i>", "*")
            .Replace("<b>", "**")
            .Replace("</b>", "**");

        var value = Regex.Replace(supportedTagsReplaced, "<.*?>", string.Empty);
        CheckValidity(value);

        return new ClassifiedAdTitle(value);
    }

    // Implicit operator, der konverterer en ClassifiedAdTitle til en string
    public static implicit operator string(ClassifiedAdTitle title)
    {
        return title.Value;
    }

    private static void CheckValidity(string value)
    {
        if (value.Length > 100)
            throw new ArgumentOutOfRangeException(
                "Title cannot be longer that 100 characters",
                nameof(value));
    }
}