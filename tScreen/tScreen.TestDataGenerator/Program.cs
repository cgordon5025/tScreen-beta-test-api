

using System.Formats.Asn1;
using System.Globalization;
using System.Reflection;
using Application.CsvFiles.Student;
using Application.CsvFiles.User;
using Core.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace TweenScreen.TestDataGenerator;

public class Program
{
    public static void Main(string[] args)
    {
        var startDate = DateTime.Today;
        var defaultDob = DateTime.Parse("2008-01-01");

        var homeRooms = new[]
        {
            1000,
            1002,
            2008,
            2010,
            3000,
            3050,
            5000,
            6000,
            7777
        };

        var jobTitles = new[]
        {
            ("Vice principal", 0.05f),
            ("Guidance counselor", 0.08f),
            ("Teaching assistant", 0.22f),
            ("Teacher", 0.65f),
        };

        var colors = Enum.GetNames(typeof(System.Drawing.KnownColor))
            .Skip(27).Select(x => x.ToWordCase(true)).ToArray();

        var userGenerator = new Bogus.Faker<BulkUserFile>()
            .RuleFor(m => m.FirstName, f => f.Person.FirstName)
            .RuleFor(m => m.MiddleName, _ => Faker.Name.Middle())
            .RuleFor(m => m.LastName, f => f.Person.LastName)
            .RuleFor(m => m.Email, (f, m) =>
                f.Internet.Email(m.FirstName, m.LastName, "biguat.futuresthrive.com"))
            .RuleFor(m => m.StartDate, _ => startDate)
            .RuleFor(m => m.EndDate, _ => DateTime.Today.AddYears(7))
            .RuleFor(m => m.JobTitle, f => f.IndexFaker == 0
                ? "Principal"
                : f.Random.WeightedRandom(
                    jobTitles.Select(x => x.Item1).ToArray(),
                    jobTitles.Select(x => x.Item2).ToArray()))
            .RuleFor(m => m.UserRole, (f, m) =>
                new[] { "Principal", "Vice principal" }.Contains(m.JobTitle) ? "Admin" : "User");

        var users = userGenerator.UseSeed(777).Generate(20);

        var emails = users.Select(e => e.Email).ToArray();

        var studentGenerator = new Bogus.Faker<BulkStudentFile>()
            .RuleFor(m => m.FirstName, f => f.Person.FirstName)
            .RuleFor(m => m.MiddleName, f => f.Random.Int(0, 2) == 0 ? Faker.Name.Middle() : null)
            .RuleFor(m => m.LastName, f => f.Person.LastName)
            .RuleFor(m => m.UserEmails, f => f.PickRandom(emails, f.Random.Int(1, 10)).ToArray())
            .RuleFor(m => m.ParentPhone, f =>
            {
                f.Phone.Locale = "en_US";
                // Sometimes Bogus.Faker will produce phone numbers that are not valid
                // when this is the case, we'll generate another phone number until a
                // valid number is produced
                while (true)
                {
                    var phoneNumber = f.Phone.PhoneNumber();
                    var phoneNumberUtil = PhoneNumbers.PhoneNumberUtil.GetInstance();
                    var phoneNumberData = phoneNumberUtil.Parse(phoneNumber, "US");

                    if (phoneNumberUtil.IsValidNumberForRegion(phoneNumberData, "US"))
                        return phoneNumber;
                }

            })
            .RuleFor(m => m.ParentEmail, (f, m) => Faker.Internet.Email($"{m.FirstName} {m.LastName}"))
            .RuleFor(m => m.Dob, _ => defaultDob)
            .RuleFor(m => m.Grade, f => f.Random.Int(4, 7))
            .RuleFor(m => m.PostalCode, f => f.Address.ZipCode())
            .RuleFor(m => m.StartDate, _ => startDate)
            .RuleFor(m => m.EndDate, _ => DateTime.Today.AddYears(12))
            .RuleFor(m => m.CustomFieldValueOne, f => f.Random.ListItem(colors))
            .RuleFor(m => m.CustomFieldValueTwo, f => f.Random.ListItem(homeRooms).ToString());

        var students = studentGenerator.UseSeed(777).Generate(1000);

        // Get the name attributes associated with the model properties. We'll use these
        // to create the CSV header.
        var userBulkFileNameAttributes = typeof(BulkUserFile)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(info => info
                .GetCustomAttributes(typeof(CsvHelper.Configuration.Attributes.NameAttribute), false)
                .FirstOrDefault())
            .Cast<CsvHelper.Configuration.Attributes.NameAttribute>()
            .ToList();

        var studentBulkFileNameAttributes = typeof(BulkStudentFile)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(info => info
                .GetCustomAttributes(typeof(CsvHelper.Configuration.Attributes.NameAttribute), false)
                .FirstOrDefault())
            .Cast<CsvHelper.Configuration.Attributes.NameAttribute>()
            .ToList();

        var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Encoding = System.Text.Encoding.UTF8
        };

        using var userStreamWriter = new StreamWriter("biguat-persons.csv");
        using var userCsv = new CsvWriter(userStreamWriter, csvConfiguration);

        userCsv.Context.RegisterClassMap<BulkUserFileMap>();

        foreach (var header in userBulkFileNameAttributes)
        {
            userCsv.WriteField(header.Names.First().ToSnakeCase());
        }

        userCsv.NextRecord();

        foreach (var record in users)
        {
            userCsv.WriteRecord(record);
            userCsv.NextRecord();
        }

        using var studentStreamWriter = new StreamWriter("biguat-students.csv");
        using var studentCsv = new CsvWriter(studentStreamWriter, csvConfiguration);

        studentCsv.Context.RegisterClassMap<BulkStudentFileMap>();

        // Write headers 
        foreach (var header in studentBulkFileNameAttributes)
        {
            studentCsv.WriteField(header.Names.First().ToSnakeCase());
        }

        studentCsv.NextRecord();

        foreach (var record in students)
        {
            studentCsv.WriteRecord(record);
            studentCsv.NextRecord();
        }
    }
}

