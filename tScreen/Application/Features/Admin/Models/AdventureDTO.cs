using System;
using System.Collections.Generic;
using Application.Common.Models;

namespace Application.Features.Admin.Models;

public class AdventureDTO : BaseEntityDTO
{
    public Guid FileId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int Position { get; set; }

    public CoreFileDTO? CoreFile { get; set; }

    public ICollection<SessionDTO> Sessions { get; set; } = new HashSet<SessionDTO>();
    public ICollection<SceneDTO> Scenes { get; set; } = new HashSet<SceneDTO>();
}