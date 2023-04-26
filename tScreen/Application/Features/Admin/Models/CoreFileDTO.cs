using System.Collections.Generic;
using Application.Common.Models;

namespace Application.Features.Admin.Models;

public class CoreFileDTO : BaseEntityDTO
{
    public string? Category { get; set; }
    public string? MimeType { get; set; }
    public string? BlobName { get; set; }
    public string? FileName { get; set; }
    public long FileSize { get; set; }
    public string? FileHash { get; set; }
    public string? BlurHash { get; set; }
    public string? Formats { get; set; }
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public string? StorageAccount { get; set; }
    public string? StorageContainer { get; set; }

    public ICollection<AvatarDTO> AvatarBodies { get; set; } = new HashSet<AvatarDTO>();
    public ICollection<AvatarDTO> AvatarEyes { get; set; } = new HashSet<AvatarDTO>();
    public ICollection<AvatarDTO> AvatarHair { get; set; } = new HashSet<AvatarDTO>();
    public ICollection<AvatarDTO> AvatarOutfits { get; set; } = new HashSet<AvatarDTO>();
    public ICollection<AvatarDTO> AvatarHelpers { get; set; } = new HashSet<AvatarDTO>();
    public ICollection<AdventureDTO> Adventures { get; set; } = new HashSet<AdventureDTO>();
}