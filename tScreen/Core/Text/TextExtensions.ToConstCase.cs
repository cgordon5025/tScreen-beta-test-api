namespace Core.Text;

public static partial class TextExtensions
{
    private const char ConstCaseDelimiter = '_';

    public static string ToConstCase(this string text) =>
        CaseTransformer(text, ConstCaseDelimiter, (c, isFrontDelimiterDisabled, _) =>
        {
            return isFrontDelimiterDisabled
                ? new[] { c }
                : new[] { ConstCaseDelimiter, c };
        }).ToUpperInvariant();
}