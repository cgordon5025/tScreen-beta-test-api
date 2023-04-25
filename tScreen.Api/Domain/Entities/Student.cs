using System;
using System.Collections.Generic;
using Domain.Common;
using Domain.Entities.App;

// ReSharper disable CollectionNeverUpdated.Global

namespace Domain.Entities
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Student : BaseEntity
    {
        public Student () {}
        
        public Student(string? firstName, string? middleName, string? lastName, string? email, DateTime dob)
        {
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            Email = email;
            Dob = dob;
        }

        public Guid LocationId { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? ParentPhone { get; set; }
        public string? ParentEmail { get; set; }
        public DateTime Dob { get; set; }
        public string? PostalCode { get; set; }
        public int? GradeLevel { get; set; }
        public bool MinimumProvided { get; set; }

        public string FullName => !string.IsNullOrWhiteSpace(MiddleName)
            ? $"{FirstName} {MiddleName} {LastName}"
            : $"{FirstName} {LastName}";
        
        public ICollection<Avatar> Avatars { get; set; } = new HashSet<Avatar>();
        public ICollection<Location> Locations { get; set; } = new HashSet<Location>();
        
        public ICollection<Session> Sessions { get; set; } = new HashSet<Session>();
        public ICollection<PersonStudent> PersonStudents { get; set; } = new HashSet<PersonStudent>();
        public ICollection<StudentCustomField> StudentCustomFields { get; set; } = new HashSet<StudentCustomField>();
        public ICollection<HistoryStudent> HistoryStudents { get; set; } = new HashSet<HistoryStudent>();
    }
}