using Application.CsvFiles.Converters;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using DateTimeConverter = Application.CsvFiles.Converters.DateTimeConverter;

namespace Application.CsvFiles.Student;

public sealed class BulkStudentFileMap : ClassMap<BulkStudentFile>
{
    public BulkStudentFileMap()
    {
        Map(c => c.FirstName).Index(0);
        Map(c => c.MiddleName).Index(1);
        Map(c => c.LastName).Index(2);
        Map(c => c.Dob).Index(3).TypeConverter<DateTimeConverter>();
        Map(c => c.PostalCode).Index(4);
        Map(c => c.UserEmails).Index(5).TypeConverter<ListConverter>();
        Map(c => c.ParentPhone).Index(6);
        Map(c => c.ParentEmail).Index(7);
        Map(c => c.Grade).Index(8);
        Map(c => c.StartDate).Index(9).TypeConverter<DateTimeConverter>();
        Map(c => c.EndDate).Index(10).TypeConverter<DateTimeConverter>().Optional();
        Map(c => c.CustomFieldValueOne).Index(11);
        Map(c => c.CustomFieldValueTwo).Index(12);
    }
}