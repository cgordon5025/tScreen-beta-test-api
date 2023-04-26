using System;
using System.Collections.Generic;
using Domain.Interfaces;
using HotChocolate.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace GraphQl.GraphQl.Models
{
    [Authorize]
    public class Person : BaseEntity, IEntityData
    {
        public Guid CompanyId { get; set; }

        public string FirstName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = null!;
        public string? JobTitle { get; set; }
        public DateTime? Dob { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public Company? Company { get; set; }

        public ICollection<Session> Sessions { get; set; } = new HashSet<Session>();
        public ICollection<PersonStudent> PersonStudents { get; set; } = new HashSet<PersonStudent>();
        public ICollection<LocationPerson> LocationPersons { get; set; } = new HashSet<LocationPerson>();
        public ICollection<WorkList> WorkLists { get; set; } = new HashSet<WorkList>();
        public ICollection<HistoryPerson> HistoryPersons { get; set; } = new HashSet<HistoryPerson>();
    }
}