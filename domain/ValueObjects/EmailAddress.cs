using System.Text.RegularExpressions;

namespace Nubulus.Domain.ValueObjects;

public class EmailAddress
{
    public string Value { get; set; }

    public EmailAddress(string address)
    {
        Value = address;
    }
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Value) || !Value.Contains("@"))
        {
            throw new ArgumentException("Invalid email address.");
        }
        if (Value.Length > 254)
        {
            throw new ArgumentException("Email address is too long.");
        }

        string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        bool isValid = Regex.IsMatch(Value, pattern, RegexOptions.IgnoreCase);
        if (!isValid)
        {
            throw new ArgumentException("Email address format is invalid.");
        }
    }
}
