using System;
using Application.Common.Models;

namespace Application.Features.Admin.Models
{
    public class LocationDTO : BaseEntityDTO
    {
        public Guid CompanyId { get; set; }
        public string? Type { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? StreetLineOne { get; set; }
        public string? StreetLineTwo { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }

        public CompanyDTO? Company { get; set; }
    }
}