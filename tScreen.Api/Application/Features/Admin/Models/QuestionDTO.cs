using System.Collections.Generic;
using Application.Common.Models;

namespace Application.Features.Admin.Models;

public class QuestionDTO : BaseEntityDTO
{
    public string? Category { get; set; }
    public string? Type { get; set; }
    public string? Title { get; set; }
    public string? Body { get; set; }
    public string? Data { get; set; }
    public int Position { get; set; }

    public ICollection<AnswerDTO> Answers { get; set; } = new HashSet<AnswerDTO>();
    public ICollection<SceneQuestionDTO> SceneQuestions { get; set; } = new HashSet<SceneQuestionDTO>();
    public ICollection<QuestionContingentDTO> QuestionContingents { get; set; } = new HashSet<QuestionContingentDTO>();
    public ICollection<QuestionContingentDTO> QuestionParentContingents { get; set; } = new HashSet<QuestionContingentDTO>();
}