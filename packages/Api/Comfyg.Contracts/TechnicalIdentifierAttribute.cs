using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Comfyg.Contracts;

internal class TechnicalIdentifierAttribute : ValidationAttribute
{
    // https://learn.microsoft.com/en-us/rest/api/storageservices/Understanding-the-Table-Service-Data-Model#characters-disallowed-in-key-fields
    private static readonly byte[] DisallowedCharacters =
        "/\\#?\t\n\r"u8.ToArray().Concat(new byte[32].Select((_, i) => (byte)(127 + i))).ToArray();

    public override bool IsValid(object? value)
    {
        if (value == null) return false;

        if (value is not string s) throw new ValidationException("Only strings can be used as technical identifier.");

        return Encoding.UTF8.GetBytes(s).All(b => !DisallowedCharacters.Contains(b));
    }

    public override string FormatErrorMessage(string name)
    {
        return $"The field {name} is a technical identifier and contains disallowed characters.";
    }
}
