namespace Core.Text;

public static partial class TextExtensions
{
    private const char ColonDelimiter = ':';
    public static string ToKeyVaultSecretName(this string text) => 
        CaseTransformer(text, ColonDelimiter, (c, _, _) =>
        {
            return new[] { '-', '-', char.ToLowerInvariant(c) };
        });
}