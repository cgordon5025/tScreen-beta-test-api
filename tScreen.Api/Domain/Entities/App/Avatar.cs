using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;

namespace Domain.Entities.App
{
    [Table(nameof(Avatar), Schema = Schema.TweenScreenApp)]
    public class Avatar : BaseEntity
    {
        public Guid StudentId { get; set; }
        public string Type { get; set; } = AvatarTypes.SystemDefined;
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

        public Core.CoreFile? BodyCoreFile { get; set; }
        public Core.CoreFile? EyeCoreFile { get; set; }
        public Core.CoreFile? HairCoreFile { get; set; }
        public Core.CoreFile? OutfitCoreFile { get; set; }
        public Core.CoreFile? HelperCoreFile { get; set; }

        public ICollection<Session> Sessions { get; set; } = new HashSet<Session>();
    }

    public static class AvatarTypes
    {
        public const string SystemDefined = nameof(SystemDefined);
        public const string UserDefined = nameof(UserDefined);
    }
}