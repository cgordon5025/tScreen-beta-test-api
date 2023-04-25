using System;
using System.Collections.Generic;
using Application.Common.Models;

namespace Application.Features.Admin.Models;

public class SceneDTO : BaseEntityDTO
{
    public Guid AdventureId { get; set; }  
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int Position { get; set; }

    public AdventureDTO? Adventure { get; set; }

    public ICollection<SceneQuestionDTO> SceneQuestions { get; set; } = new HashSet<SceneQuestionDTO>();
}