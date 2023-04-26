using Application.CsvFiles;
using Application.CsvFiles.User;
using Application.Features.Admin.Models;
using AutoMapper;
using GraphQl.GraphQl.Models;

namespace GraphQl.GraphQl.Features
{
    public class GraphQlMapProfile : Profile
    {
        public GraphQlMapProfile()
        {
            CreateMap<Domain.Entities.Company, Company>();
            CreateMap<Domain.Entities.Location, Location>();
            CreateMap<Domain.Entities.Person, Person>();
            CreateMap<Domain.Entities.Student, Student>();
            CreateMap<Domain.Entities.PersonStudent, PersonStudent>();
            CreateMap<Domain.Entities.CustomField, CustomField>();
            CreateMap<Domain.Entities.StudentCustomField, StudentCustomField>();
            CreateMap<Domain.Entities.WorkList, WorkList>();
            CreateMap<WorkListDTO, WorkList>();
            CreateMap<Domain.Entities.WorkListNote, WorkListNote>();
            CreateMap<WorkListNoteDTO, WorkListNote>();
            CreateMap<Domain.Entities.SessionNote, SessionNote>();
            CreateMap<Domain.Entities.Note, Note>();
            CreateMap<NoteDTO, Note>();
            CreateMap<Domain.Entities.History, History>();
            CreateMap<Domain.Entities.File, File>();
            CreateMap<Domain.Entities.App.File, File>();
            CreateMap<Domain.Entities.HistoryWorkList, HistoryWorkList>();
            CreateMap<Domain.Entities.HistoryStudent, HistoryStudent>();
            CreateMap<Domain.Entities.HistoryPerson, HistoryPerson>();
            CreateMap<Domain.Entities.LocationPerson, LocationPerson>();

            CreateMap<CustomFieldDTO, CustomField>();
            CreateMap<StudentDTO, Student>();
            CreateMap<StudentCustomFieldDTO, StudentCustomField>();

            CreateMap<Domain.Entities.Core.Adventure, Adventure>();
            CreateMap<Domain.Entities.Core.CoreFile, CoreFile>();
            CreateMap<Domain.Entities.Core.Scene, Scene>();
            CreateMap<Domain.Entities.Core.SceneQuestion, SceneQuestion>();
            CreateMap<Domain.Entities.Core.Question, Question>();
            CreateMap<Domain.Entities.Core.QuestionContingent, QuestionContingent>();

            CreateMap<Domain.Entities.App.Answer, Answer>();
            CreateMap<AnswerDTO, Answer>();
            CreateMap<Domain.Entities.App.Avatar, Avatar>();
            CreateMap<AvatarDTO, Avatar>();
            CreateMap<Domain.Entities.App.Session, Session>();
            CreateMap<SessionDTO, Session>();

            CreateMap<PersonDTO, BulkUserFile>().ReverseMap();
        }
    }
}