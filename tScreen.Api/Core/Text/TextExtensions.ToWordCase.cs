namespace Core.Text;

public static partial class TextExtensions
{
    private const char WordCaseDelimiter = ' ';

    public static string ToWordCase(this string text, bool capitalize = false) =>
        CaseTransformer(text, WordCaseDelimiter, (c, isFrontDelimiterDisabled, charIndex) =>
        {
            return isFrontDelimiterDisabled 
                ? new[] { capitalize && charIndex == 0 ? char.ToUpperInvariant(c) : char.ToLowerInvariant(c) } 
                : new[] { WordCaseDelimiter, capitalize ? char.ToUpperInvariant(c) : char.ToLowerInvariant(c) };
        });
}