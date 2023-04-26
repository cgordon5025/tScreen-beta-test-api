using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Application.Common.Models;
using Domain.Entities;

namespace Application.Features.Admin.Models
{
    public class PersonDTO : BaseEntityDTO
    {
        public Guid CompanyId { get; set; }
        public string? IdentityId { get; set; }
        public string? IdentityType { get; set; }
        public string FirstName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = null!;
        public DateTime? Dob { get; set; }
        public string? JobTitle { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [NotMapped]
        public CompanyDTO? Company { get; set; }

        [NotMapped]
        public ICollection<LocationPersonDTO> LocationPersons { get; set; } = new HashSet<LocationPersonDTO>();
    }
}