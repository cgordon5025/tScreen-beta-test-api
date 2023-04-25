using System;
using System.Collections.Generic;
using Application.Common.Models;

namespace Application.Features.Admin.Models
{
    public class StudentDTO : BaseEntityDTO
    {
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
        
        public IEnumerable<CustomFieldPair>? CustomFieldPairs { get; set; }

        public ICollection<StudentCustomFieldDTO> StudentCustomFields { get; set; } = new HashSet<StudentCustomFieldDTO>();
    }

    public class CustomFieldPair
    {
        public Guid Id { get; set; }
        public string? Value { get; set; }
    }
}