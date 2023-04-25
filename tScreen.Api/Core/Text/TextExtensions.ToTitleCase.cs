using System.Globalization;

namespace Core.Text;

public static partial class TextExtensions
{
    public static string ToTitleCase(this string text)
    {
        var textInfo = new CultureInfo("en-US", false).TextInfo;
        return textInfo.ToTitleCase(text);
    }
}