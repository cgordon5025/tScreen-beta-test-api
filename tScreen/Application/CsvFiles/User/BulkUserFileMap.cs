using Application.CsvFiles.Converters;
using CsvHelper.Configuration;

namespace Application.CsvFiles.User;

public sealed class BulkUserFileMap : ClassMap<BulkUserFile>
{
    public BulkUserFileMap()
    {
        Map(c => c.FirstName).Index(0);
        Map(c => c.MiddleName).Index(1);
        Map(c => c.LastName).Index(2);
        Map(c => c.UserRole).Index(3);
        Map(c => c.Email).Index(4);
        Map(c => c.JobTitle).Index(5);
        Map(c => c.StartDate).Index(6).TypeConverter<DateTimeConverter>();
        Map(c => c.EndDate).Index(7).Optional().TypeConverter<DateTimeConverter>();
    }
}