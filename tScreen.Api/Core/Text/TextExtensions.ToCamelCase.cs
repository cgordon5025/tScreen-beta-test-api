namespace Core.Text;

public static partial class TextExtensions
{
    private const char CamelCaseDelimiter = '\0';
    
    public static string ToCamelCase(this string text) => 
        CaseTransformer(text, CamelCaseDelimiter, (c, isFrontDelimiterDisabled, _) =>
        {
            return isFrontDelimiterDisabled 
                ? new[] { char.ToUpperInvariant(c) } 
                : new[] { char.ToLowerInvariant(c) };
        });
}