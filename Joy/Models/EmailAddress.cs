using System.Text.RegularExpressions;

namespace AvP.Joy.Models
{
    public sealed class EmailAddress : StringWrapper
    {
        // https://www.oreilly.com/library/view/regular-expressions-cookbook/9781449327453/ch04s01.html
        private static readonly Parser<EmailAddress> parser = Parser.Of(
            value => new EmailAddress(value),
            new Regex("^[\\w!#$%&'*+/=?`{|}~^-]+(?:\\.[\\w!#$%&'*+/=?`{|}~^-]+)*@↵\r\n(?:[A-Z0-9-]+\\.)+[A-Z]{2,6}$", RegexOptions.IgnoreCase));

        private EmailAddress(string value) : base(value) { }

        public static EmailAddress Parse(string value) => parser.Parse(value);
        public static bool TryParse(string value, out EmailAddress success) => parser.TryParse(value, out success);
        public static bool CanParse(string value) => parser.CanParse(value);

        public static bool operator ==(EmailAddress a, EmailAddress b) => object.Equals(a, b);
        public static bool operator !=(EmailAddress a, EmailAddress b) => !object.Equals(a, b);
    }
}
