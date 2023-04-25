using System;
using Application.Common.Models;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Admin.Models;

public class FileDTO : BaseEntityDTO
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
    public IFormFile? File { get; set; } 
    
    public SessionDTO? Session { get; set; }
    public LocationDTO? Location { get; set; }
}