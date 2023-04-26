using System;
using CsvHelper.Configuration.Attributes;

namespace Application.CsvFiles.User;

public class BulkUserFile
{
    [Name("first-name")]
    public string FirstName { get; set; } = null!;

    [Name("middle-name")]
    public string MiddleName { get; set; } = null!;

    [Name("last-name")]
    public string LastName { get; set; } = null!;

    [Name("role")]
    public string UserRole { get; set; } = null!;

    [Name("email")]
    public string Email { get; set; } = null!;

    [Name("job-title")]
    public string? JobTitle { get; set; }

    [Name("start-date")]
    public DateTime StartDate { get; set; }

    [Name("end-date")]
    public DateTime EndDate { get; set; }
}