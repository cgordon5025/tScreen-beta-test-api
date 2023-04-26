using Application.Features.Admin.Models;
using AutoMapper;
using Domain.Entities.Core;

namespace Application.Mappings;

public class CoreMapProfile : Profile
{
    public CoreMapProfile()
    {
        CreateMap<Adventure, AdventureDTO>().ReverseMap();
        CreateMap<CoreFile, CoreFileDTO>().ReverseMap();
        CreateMap<Question, QuestionDTO>().ReverseMap();
        CreateMap<QuestionContingent, QuestionContingent>().ReverseMap();
        CreateMap<Scene, SceneDTO>().ReverseMap();
        CreateMap<SceneQuestion, SceneQuestionDTO>().ReverseMap();
    }
}