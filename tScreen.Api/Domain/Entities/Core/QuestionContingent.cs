using System;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;

namespace Domain.Entities.Core
{
    [Table(nameof(QuestionContingent), Schema = Schema.TweenScreenCore)]
    public class QuestionContingent : BaseEntity
    {
        public Guid ParentId { get; set; }
        public Guid QuestionId { get; set; }
        public int? Position { get; set; }
        public string? Rule { get; set; }
        
        public Question? Question { get; set; }
        public Question? QuestionParent { get; set; }
    }
}