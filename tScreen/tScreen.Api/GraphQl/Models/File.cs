using System;
using HotChocolate.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace GraphQl.GraphQl.Models
{
    [Authorize]
    public class File : BaseEntity
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

        public Location? Location { get; set; }
    }
}