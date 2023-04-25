using System;
using System.Collections.Generic;
using HotChocolate.AspNetCore.Authorization;

namespace GraphQl.GraphQl.Models
{
    [Authorize]
    public class CoreFile : BaseEntity
    {
        public Guid LocationId { get; set; }
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

        public ICollection<Avatar> AvatarBodies { get; set; } = new HashSet<Avatar>();
        public ICollection<Avatar> AvatarEyes { get; set; } = new HashSet<Avatar>();
        public ICollection<Avatar> AvatarHair { get; set; } = new HashSet<Avatar>();
        public ICollection<Avatar> AvatarOutfits { get; set; } = new HashSet<Avatar>();
        public ICollection<Avatar> AvatarHelpers { get; set; } = new HashSet<Avatar>();
        public ICollection<Adventure> Adventures { get; set; } = new HashSet<Adventure>();
    }
}