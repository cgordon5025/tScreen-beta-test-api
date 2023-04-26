using System;
using HotChocolate.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace GraphQl.GraphQl.Models
{
    [Authorize]
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