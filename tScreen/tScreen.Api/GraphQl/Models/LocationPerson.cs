using System;
using HotChocolate.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace GraphQl.GraphQl.Models
{
    [Authorize]
    public class LocationPerson : BaseEntity
    {
        public Guid LocationId { get; set; }
        public Guid PersonId { get; set; }
        public string? Type { get; set; }

        public Location? Location { get; set; }
        public Person? Person { get; set; }
    }
}