using System.IO;

namespace Core.Text;

public static partial class TextExtensions
{
    public static string ToPathCase(this string text, bool capitalize = false)
    {
        var pathSeparator = Path.PathSeparator;
        return CaseTransformer(text, pathSeparator, (c, isFrontDelimiterDisabled, charIndex) =>
        {
            return isFrontDelimiterDisabled
                ? new[] { capitalize && charIndex == 0 ? char.ToUpperInvariant(c) : char.ToLowerInvariant(c) }
                : new[] { pathSeparator, capitalize ? char.ToUpperInvariant(c) : char.ToLowerInvariant(c) };
        });
    }
}