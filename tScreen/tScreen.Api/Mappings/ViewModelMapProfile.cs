using Application.Common.Models;
using AutoMapper;
using GraphQl.Models.SessionController;

namespace GraphQl.Mappings;

public class ViewModelMapProfile : Profile
{
    public ViewModelMapProfile()
    {
        CreateMap<AuthorizeResultDTO, AuthenticateResponse>();
    }
}