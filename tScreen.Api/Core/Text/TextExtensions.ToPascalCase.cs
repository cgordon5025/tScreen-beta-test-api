namespace Core.Text;

public static partial class TextExtensions
{
    private const char PascalCaseDelimiter = '\0';
    
    public static string ToPascalCase(this string text) => 
        CaseTransformer(text, PascalCaseDelimiter, (c, _, _) =>
        {
            return new [] { char.ToUpperInvariant(c) };
        });
}