using System.Text.RegularExpressions;

namespace Marketplace.Domain.ValueObjects
{
    public record ClassifiedAdTitle
    {
        public string Value { get; }

        private ClassifiedAdTitle(string value)
        {
            if (value.Length > 100)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value), "Title cannot be longer than 100 characters");
            }

            Value = value;
        }

        public override string ToString() => Value;

        // Factory method, der konverterer en string til en ClassifiedAdTitle
        public static ClassifiedAdTitle FromString(string title) => new ClassifiedAdTitle(title);

        // Factory method, der konverterer en html-streng til en ClassifiedAdTitle
        public static ClassifiedAdTitle FromHtml(string htmlTitle)
        {
            var supportedTagsReplaced = htmlTitle
                .Replace("<i>", "*")
                .Replace("</i>", "*")
                .Replace("<b>", "**")
                .Replace("</b>", "**");

            return new ClassifiedAdTitle(
                Regex.Replace(supportedTagsReplaced, "<.*.>", string.Empty));
        }

        private static void CheckValidity(string value)
        {
            if (value.Length > 100)
                throw new ArgumentOutOfRangeException(
                    "Title cannot be longer that 100 characters",
                    nameof(value));
        }

        public static implicit operator string(ClassifiedAdTitle title) =>
            title.Value;



    }
}