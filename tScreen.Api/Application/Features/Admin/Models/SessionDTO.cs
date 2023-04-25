using System;
using System.Collections.Generic;
using Application.Common.Models;

namespace Application.Features.Admin.Models;

public class SessionDTO : BaseEntityDTO
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
    public object? Data { get; set; }

    public StudentDTO? Student { get; set; }
    public PersonDTO? Person { get; set; }
    public AdventureDTO? Adventure { get; set; }
    public AvatarDTO? Avatar { get; set; }

    public ICollection<AnswerDTO> Answers { get; set; } = new HashSet<AnswerDTO>();
    public ICollection<SessionNoteDTO> SessionNotes { get; set; } = new HashSet<SessionNoteDTO>();
}

public class SessionDataDTO
{
    public SessionDataTagDTO? Tags { get; set; }
}

public class SessionDataTagDTO
{
    public string[] Ace { get; set; } = Array.Empty<string>();
    public string[] Pce { get; set; } = Array.Empty<string>();
}