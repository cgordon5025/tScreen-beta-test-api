using System;
using HotChocolate.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace GraphQl.GraphQl.Models
{
    [Authorize]
    public class PersonStudent : BaseEntity
    {
        public Guid PersonId { get; set; }
        public Guid StudentId { get; set; }

        public Person? Person { get; set; }
        public Student? Student { get; set; }
    }
}