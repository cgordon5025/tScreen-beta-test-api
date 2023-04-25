using System;
using Domain.Common;

namespace Domain.Entities
{
    public class PersonStudent : BaseEntity
    {
        public Guid PersonId { get; set; }
        public Guid StudentId { get; set; }
        
        public Person? Person { get; set; }
        public Student? Student { get; set; }
    }
}