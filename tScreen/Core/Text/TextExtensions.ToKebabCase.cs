namespace Core.Text;

public static partial class TextExtensions
{
    private const char KebabDelimiter = '-';

    public static string ToKebabCase(this string text, bool capitalize = false) =>
        CaseTransformer(text, KebabDelimiter, (c, isFrontDelimiterDisabled, charIndex) =>
        {
            return isFrontDelimiterDisabled
                ? new[] { capitalize && charIndex == 0 ? char.ToUpperInvariant(c) : char.ToLowerInvariant(c) }
                : new[] { KebabDelimiter, capitalize ? char.ToUpperInvariant(c) : char.ToLowerInvariant(c) };
        });
}