namespace Core.Text;

public static partial class TextExtensions
{
    public static string ToAzureStorageName(this string text) => 
        CaseTransformer(text, NullDelimiter, (c, _, _) =>
        {
            return new[] { char.ToLowerInvariant(c) };
        });
}