using System;
using System.Collections.Generic;
using GraphQl.GraphQl.Interfaces;
using HotChocolate.AspNetCore.Authorization;

namespace GraphQl.GraphQl.Models
{
    [Authorize]
    public class Avatar : BaseEntity, IAvatarResult
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
        
        public Student? Student { get; set; }

        public CoreFile? BodyCoreFile { get; set; }
        public CoreFile? EyeCoreFile { get; set; }
        public CoreFile? HairCoreFile { get; set; }
        public CoreFile? OutfitCoreFile { get; set; }
        public CoreFile? HelperCoreFile { get; set; }

        public ICollection<Session>? Sessions { get; set; } = new HashSet<Session>();
    }
}