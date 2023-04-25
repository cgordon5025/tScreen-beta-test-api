using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;
using Domain.Entities.App;

namespace Domain.Entities.Core
{
    [Table("File", Schema = Schema.TweenScreenCore)]
    public class CoreFile : BaseEntity
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

        public ICollection<Avatar> AvatarBodies { get; set; } = new HashSet<Avatar>();
        public ICollection<Avatar> AvatarEyes { get; set; } = new HashSet<Avatar>();
        public ICollection<Avatar> AvatarHair { get; set; } = new HashSet<Avatar>();
        public ICollection<Avatar> AvatarOutfits { get; set; } = new HashSet<Avatar>();
        public ICollection<Avatar> AvatarHelpers { get; set; } = new HashSet<Avatar>();
        public ICollection<Adventure> Adventures { get; set; } = new HashSet<Adventure>();
    }
}