using System;
using System.Collections.Generic;
using Domain.Common;

namespace Domain.Entities
{
    public class CustomField : BaseEntity
    {
        public Guid LocationId { get; set; }
        public string? Type { get; set; }
        public int Position { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? PlaceHolder { get; set; }
        public string? DefaultValue { get; set; }
        public string? ValidationRule { get; set; }
        
        public Location? Location { get; set; }

        public ICollection<StudentCustomField> StudentCustomFields { get; set; } = new HashSet<StudentCustomField>();
    }
}