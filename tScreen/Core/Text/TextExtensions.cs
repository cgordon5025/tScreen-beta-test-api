using System;
using System.Collections;
using System.Text;

namespace Core.Text;

public static partial class TextExtensions
{
    private const char NullDelimiter = '\0';
    private static readonly char[] Delimiters = new[] { ' ', '-', '_' };

    private static string CaseTransformer(string text, char delimiter, Func<char, bool, int, char[]> handler)
    {
        var builder = new StringBuilder();

        var isNextWord = true;
        var isFrontDelimiterDisabled = true;

        for (var index = 0; index < text.Length; index++)
        {
            var c = text[index];
            if (c == delimiter) continue;

            if (((IList)Delimiters).Contains(c))
            {
                if (c == delimiter)
                {
                    builder.Append(c);
                    isFrontDelimiterDisabled = true;
                }

                isNextWord = true;
            }
            else if (!char.IsLetterOrDigit(c))
            {
                builder.Append(c);
                isNextWord = true;
                isFrontDelimiterDisabled = true;
            }
            else
            {
                // Disable the delimiter if the adjacent, left character is uppercase
                if (index > 0 && char.IsUpper(c) && char.IsUpper(text[index - 1]))
                    isFrontDelimiterDisabled = true;

                if (isNextWord || char.IsUpper(c))
                {
                    builder.Append(handler(c, isFrontDelimiterDisabled, index));
                    isFrontDelimiterDisabled = false;
                    isNextWord = false;
                    continue;
                }

                builder.Append(c);
            }
        }

        return builder.ToString();
    }
}