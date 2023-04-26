using System;
using System.Collections.Generic;
using GraphQl.GraphQl.Features.Objects.Session.Results;
using GraphQl.GraphQl.Interfaces;
using HotChocolate.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace GraphQl.GraphQl.Models
{
    [Authorize]
    public class Session : BaseEntity, ISessionResult
    {
        public Guid StudentId { get; set; }
        public Guid PersonId { get; set; }
        public Guid LocationId { get; set; }
        public Guid? AdventureId { get; set; }
        public Guid? AvatarId { get; set; }
        public string Type { get; set; } = null!;
        public string Checkpoint { get; set; } = null!;
        public DateTime? FinishedAt { get; set; }
        public int RiskRating { get; set; }
        public string? Code { get; set; }
        public string? Data { get; set; }

        public Student? Student { get; set; }
        public Person? Person { get; set; }
        public Adventure? Adventure { get; set; }
        public Avatar? Avatar { get; set; }
        public File? File { get; set; }

        public SessionSummary? SessionSummary { get; set; }

        public ICollection<Answer>? Answers { get; set; } = new HashSet<Answer>();
        public ICollection<WorkList> WorkLists { get; set; } = new HashSet<WorkList>();
        public ICollection<SessionNote>? SessionNotes { get; set; } = new HashSet<SessionNote>();
    }
}