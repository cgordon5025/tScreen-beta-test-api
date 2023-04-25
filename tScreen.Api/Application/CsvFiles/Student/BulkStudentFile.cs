using System;
using System.Collections.Generic;
using CsvHelper.Configuration.Attributes;

namespace Application.CsvFiles.Student;

public class BulkStudentFile
{
    [Name("child-first-name")]
    public string FirstName { get; set; } = null!;
    
    [Name("child-middle-name")]
    public string? MiddleName { get; set; }
    
    [Name("child-last-name")]
    public string LastName { get; set; } = null!;
    
    [Name("dob")]
    public DateTime Dob { get; set; }
    
    [Name("postalcode")]
    public string? PostalCode { get; set; }
    
    [Name("associated-emails")]
    public IEnumerable<string> UserEmails { get; set; } = null!;
    // public string? Email { get; set; }
    
    [Name("parent-phone")]
    public string? ParentPhone { get; set; }
    
    [Name("parent-email")]
    public string? ParentEmail { get; set; }
    
    [Name("grade")]
    public int? Grade { get; set; } = default;
    
    [Name("start-date")]
    public DateTime StartDate { get; set; }
    
    [Name("end-date")]
    public DateTime? EndDate { get; set; }
    
    [Name("custom-value-one")]

    public string? CustomFieldValueOne { get; set; }
    
    [Name("custom-value-two")]
    public string? CustomFieldValueTwo { get; set; }
}