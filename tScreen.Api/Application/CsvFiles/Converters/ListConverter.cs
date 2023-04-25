using System.Collections.Generic;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace Application.CsvFiles.Converters;

public class ListConverter : DefaultTypeConverter
{
    public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        if (text.Trim().Length == 0)
            return Enumerable.Empty<string>();
        
        return text.Contains(',') 
            ? text.Split(',').Select(e => e.Trim()) 
            : new[] { text };
    }

    public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
    {
        return value is string[] list ? string.Join(",", list) : null;
    }
}