using System;
using System.Collections.Generic;
using Application.Common.Models;

namespace Application.Features.Admin.Models
{
    public class CustomFieldDTO : BaseEntityDTO
    {
        public Guid LocationId { get; set; }
        public string? Type { get; set; }
        public int Position { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? PlaceHolder { get; set; }
        public string? DefaultValue { get; set; }
        public string? ValidationRule { get; set; }
        
        public LocationDTO? Location { get; set; }

        public ICollection<StudentCustomFieldDTO> StudentCustomFields { get; set; } = new HashSet<StudentCustomFieldDTO>();
    }
}