using System.Collections.Generic;
using HotChocolate.AspNetCore.Authorization;

namespace GraphQl.GraphQl.Models
{
    /// <summary>
    /// Represents a physical company 
    /// </summary>
    [Authorize]
    public class Company : BaseEntity
    {
        public string? Type { get; set; }
        public string? Name { get; set; }
        public string? Slug { get; set; }
        public string? Description { get; set; }

        public ICollection<Location> Locations { get; set; } = new HashSet<Location>();
        public ICollection<Person> Persons { get; set; } = new HashSet<Person>();
    }

    public static class CompanyType
    {
        public const string School = nameof(School);
        public const string PrivateClinic = nameof(PrivateClinic);
        public const string Hospital = nameof(Hospital);
    }
}