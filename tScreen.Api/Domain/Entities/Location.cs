using System;
using System.Collections.Generic;
using Domain.Common;

namespace Domain.Entities
{
    public class Location : BaseEntity
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
        
        public Company? Company { get; set; }
        public Student? Student { get; set; }

        public ICollection<File> Files { get; set; } = new HashSet<File>();

        public ICollection<CustomField> CustomFields { get; set; } = new HashSet<CustomField>();
        public ICollection<LocationPerson> LocationPersons { get; set; } = new HashSet<LocationPerson>();
    }

    public static class LocationType
    {
        public const string Default = nameof(Default);
    }
}