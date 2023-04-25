namespace Core.Text;

public static partial class TextExtensions
{
    private const char PeriodCaseDelimiter = '_';

    public static string ToPeriodCase(this string text, bool capitalize = false) =>
        CaseTransformer(text, PeriodCaseDelimiter, (c, isFrontDelimiterDisabled, charIndex) =>
        {
            return isFrontDelimiterDisabled 
                ? new[] { capitalize && charIndex == 0 ? char.ToUpperInvariant(c) : char.ToLowerInvariant(c) } 
                : new[] { PeriodCaseDelimiter, capitalize ? char.ToUpperInvariant(c) : char.ToLowerInvariant(c) };
        });
}