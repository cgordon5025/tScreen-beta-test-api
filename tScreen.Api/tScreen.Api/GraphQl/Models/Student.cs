using System;
using System.Collections.Generic;
using GraphQl.GraphQl.Features.Objects.Student.Results;
using HotChocolate.AspNetCore.Authorization;

namespace GraphQl.GraphQl.Models
{
    [Authorize]
    public class Student : BaseEntity, IStudentResult
    {
        public Guid LocationId { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public DateTime Dob { get; set; }
        public string? PostalCode { get; set; }
        public int? GradeLevel { get; set; } 
        public bool MinimumProvided { get; set; }

        public Models.Session? LastSession { get; set; }
        
        public ICollection<Avatar>? Avatars { get; set; } = new HashSet<Avatar>();
        public ICollection<Location>? Locations { get; set; } = new HashSet<Location>();
        public ICollection<Session>? Sessions { get; set; } = new HashSet<Session>();
        public ICollection<PersonStudent>? PersonStudents { get; set; } = new HashSet<PersonStudent>();
        public ICollection<StudentCustomField>? StudentCustomFields { get; set; } = new HashSet<StudentCustomField>();
        public ICollection<HistoryStudent>? HistoryStudents { get; set; } = new HashSet<HistoryStudent>();
    }
}