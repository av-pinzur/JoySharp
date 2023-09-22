using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace AvP.Joy.Models
{
#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
    public sealed class EmailAddress : StringWrapper, IEquatable<EmailAddress>
    {
        // Regular expression taken from
        // https://www.oreilly.com/library/view/regular-expressions-cookbook/9781449327453/ch04s01.html
        private static readonly Parser<EmailAddress> parser = Parser.Of(
            value => new EmailAddress(value),
            new Regex("^[\\w!#$%&'*+/=?`{|}~^-]+(?:\\.[\\w!#$%&'*+/=?`{|}~^-]+)*@(?:[A-Z0-9-]+\\.)+[A-Z]{2,6}$", RegexOptions.IgnoreCase));

        private EmailAddress(string value) : base(value) { }

        public static EmailAddress Parse(string value) => parser.Parse(value);
        public static bool TryParse(string value, [MaybeNullWhen(false)] out EmailAddress success) => parser.TryParse(value, out success);

        public bool Equals(EmailAddress? other) => base.Equals(other);
    }
}
