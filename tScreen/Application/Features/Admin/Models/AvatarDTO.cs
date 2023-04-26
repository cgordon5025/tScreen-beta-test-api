using System;
using System.Collections.Generic;
using Application.Common.Models;

namespace Application.Features.Admin.Models;

public class AvatarDTO : BaseEntityDTO
{
    public Guid StudentId { get; set; }
    public string Type { get; set; } = null!;
    public Guid? BodyId { get; set; }
    public string? BodyColor { get; set; }
    public Guid? EyeId { get; set; }
    public string? EyeColor { get; set; }
    public Guid? HairId { get; set; }
    public string? HairColor { get; set; }
    public Guid? OutfitId { get; set; }

    public string? ShirtColor { get; set; }
    public string? PantsColor { get; set; }
    public string? ShoesColor { get; set; }
    public Guid? HelperId { get; set; }

    public StudentDTO? Student { get; set; }

    public CoreFileDTO? BodyCoreFile { get; set; }
    public CoreFileDTO? EyeCoreFile { get; set; }
    public CoreFileDTO? HairCoreFile { get; set; }
    public CoreFileDTO? OutfitCoreFile { get; set; }
    public CoreFileDTO? HelperCoreFile { get; set; }

    public ICollection<SessionDTO> Sessions { get; set; } = new HashSet<SessionDTO>();
}