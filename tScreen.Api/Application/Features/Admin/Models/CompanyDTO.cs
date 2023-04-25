using System.Collections.Generic;
using Application.Common.Models;

namespace Application.Features.Admin.Models
{
    public class CompanyDTO : BaseEntityDTO
    {
        public string? Type { get; set; }
        public string? Name { get; set; }
        public string? Slug { get; set; }
        public string? Description { get; set; }

        public ICollection<LocationDTO> Locations { get; set; } = new HashSet<LocationDTO>();
        public ICollection<PersonDTO> Persons { get; set; } = new HashSet<PersonDTO>();
    }
}