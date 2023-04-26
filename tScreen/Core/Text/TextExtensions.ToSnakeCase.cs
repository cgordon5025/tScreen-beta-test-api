namespace Core.Text;

public static partial class TextExtensions
{
    private const char SnakeCaseDelimiter = '_';

    public static string ToSnakeCase(this string text, bool capitalize = false) =>
        CaseTransformer(text, SnakeCaseDelimiter, (c, isFrontDelimiterDisabled, charIndex) =>
        {
            return isFrontDelimiterDisabled
                ? new[] { capitalize && charIndex == 0 ? char.ToUpperInvariant(c) : char.ToLowerInvariant(c) }
                : new[] { SnakeCaseDelimiter, capitalize ? char.ToUpperInvariant(c) : char.ToLowerInvariant(c) };
        });
}