using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;
using Domain.Entities.Core;

namespace Domain.Entities.App
{
    [Table(nameof(Session), Schema = Schema.TweenScreenApp)]
    public class Session : BaseEntity
    {
        public Guid StudentId { get; set; }
        public Guid PersonId { get; set; }
        public Guid LocationId { get; set; }
        public Guid? AdventureId { get; set; }
        public Guid? AvatarId { get; set; }
        public string Type { get; set; } = SessionTypes.Full;
        public string Checkpoint { get; set; } = SessionCheckpoints.Initiated;
        public string? Code { get; set; }
        public object? Data { get; set; }
        
        public DateTime? FinishedAt { get; set; }
        public int RiskRating { get; set; }
        
        public Student? Student { get; set; }
        public Person? Person { get; set; }
        public Adventure? Adventure { get; set; }
        public Avatar? Avatar { get; set; }

        public ICollection<Answer> Answers { get; set; } = new HashSet<Answer>();
        public ICollection<File> Files { get; set; } = new HashSet<File>();
        public ICollection<SessionNote> SessionNotes { get; set; } = new HashSet<SessionNote>();
        public ICollection<HistorySession> HistorySessions { get; set; } = new HashSet<HistorySession>();
    }

    public class SessionTypes
    {
        public const string Full = nameof(Full);
        public const string Partial = nameof(Partial);
    }

    public class SessionStatus
    {
        public const string Abandoned = nameof(Abandoned);
        public const string Incomplete = nameof(Incomplete);
        public const string Pending = nameof(Pending);
        public const string Complete = nameof(Complete);
    }

    public class SessionCheckpoints
    {
        public const string Initiated = nameof(Initiated);
        public const string AvatarSelected = nameof(AvatarSelected);
        public const string AuthenticationSuccess = nameof(AuthenticationSuccess);
        public const string EnvironmentQuestions = nameof(EnvironmentQuestions);
        public const string PersonalQuestions = nameof(PersonalQuestions);
        public const string AdventureSelected = nameof(AdventureSelected);
        public const string AdventureComplete = nameof(AdventureComplete);
        public const string AppClosed = nameof(AppClosed);
        
        public const string Default = "Unknown";
        
        public static string[] ToArray() => new[]
        {
            Initiated,
            AuthenticationSuccess,
            AvatarSelected,
            EnvironmentQuestions,
            PersonalQuestions,
            AdventureSelected,
            AdventureComplete,
            AppClosed
        };

        /// <summary>
        /// Get next checkpoint defined by order of the checkpoints array
        /// see <see cref="ToArray"/>
        /// </summary>
        /// <param name="key">checkpoint name</param>
        /// <returns>Next checkpoint or Default <see cref="Default"/> </returns>
        public static string Next(string? key = default)
        {
            var checkPoints = ToArray();
			
            if (key is null)
                return checkPoints[0];
			
            var index = Array.IndexOf(checkPoints, key);
            if (index < 0)
                return Default;

            var nextIndex = index + 1;
            return nextIndex < checkPoints.Length 
                ? checkPoints[nextIndex] 
                : Default;
        }
    }
}