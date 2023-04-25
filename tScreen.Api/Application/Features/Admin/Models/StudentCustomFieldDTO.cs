using System;
using Application.Common.Models;

namespace Application.Features.Admin.Models
{
    public class StudentCustomFieldDTO : BaseEntityDTO
    {
        public Guid StudentId { get; set; }
        public Guid CustomFieldId { get; set; }
        public string? Value { get; set; }
        
        public StudentDTO? Student { get; set; }
        
        public CustomFieldDTO? CustomField { get; set; }
    }
}