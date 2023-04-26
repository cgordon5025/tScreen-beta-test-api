using Application.Features.Admin.Models;
using AutoMapper;
using Domain.Entities.App;

namespace Application.Mappings;

public class AppMapProfile : Profile
{
    public AppMapProfile()
    {
        CreateMap<Answer, AnswerDTO>().ReverseMap();
        CreateMap<Session, SessionDTO>().ReverseMap();
        CreateMap<File, FileDTO>().ReverseMap();
    }
}