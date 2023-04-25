using System;
using Domain.Common;

namespace Domain.Entities
{
    public class LocationPerson : BaseEntity
    {
        public Guid LocationId { get; set; }
        public Guid PersonId { get; set; }
        public string? Type { get; set; } 
        
        public Location? Location { get; set; }
        public Person? Person { get; set; }
    }

    public static class LocationTypes
    {
        public const string Default = nameof(Default);
    }
}