using System;
using Application.Common.Models;

namespace Application.Features.Admin.Models;

public class SceneQuestionDTO : BaseEntityDTO
{
    public Guid SceneId { get; set; }
    public Guid QuestionId { get; set; }
        
    public SceneDTO? Scene { get; set; }
    public QuestionDTO? Question { get; set; }
}