using System;
using Application.Common.Models;

namespace Application.Features.Admin.Models;

public class QuestionContingentDTO : BaseEntityDTO
{
    public Guid ParentId { get; set; }
    public Guid QuestionId { get; set; }
    public string? Type { get; set; }
    public string? Rule { get; set; }
        
    public QuestionDTO? Question { get; set; }
    public QuestionDTO? QuestionParent { get; set; }
}