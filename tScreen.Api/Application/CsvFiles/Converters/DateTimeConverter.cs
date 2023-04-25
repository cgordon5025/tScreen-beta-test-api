using System;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace Application.CsvFiles.Converters;

public class DateTimeConverter : DefaultTypeConverter
{
    private const string FileDefaultDateFormat = "M/d/yy";
    
    public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        if (string.IsNullOrEmpty(text)) return DateTime.MinValue;
        
        DateTime.TryParseExact(text, FileDefaultDateFormat, CultureInfo.InvariantCulture, 
            DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var result);
        
        return result;
    }

    public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
    {
        var dateTime = (DateTime?) value;
        return value is not null ? dateTime?.ToString(FileDefaultDateFormat) : null;
    }
}