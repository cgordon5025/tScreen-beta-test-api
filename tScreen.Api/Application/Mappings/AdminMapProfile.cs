using Application.CsvFiles.Student;
using Application.Features.Admin.Models;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.App;
using File = Domain.Entities.File;

namespace Application.Mappings
{
    public class AdminMapProfile : Profile
    {
        public AdminMapProfile()
        {
            CreateMap<Avatar, AvatarDTO>().ReverseMap();
            CreateMap<Company, CompanyDTO>().ReverseMap();
            CreateMap<Location, LocationDTO>().ReverseMap();
            CreateMap<LocationPersonDTO, LocationPerson>().ReverseMap();
            CreateMap<Student, StudentDTO>().ReverseMap();
            CreateMap<Person, PersonDTO>().ReverseMap();
            CreateMap<CustomField, CustomFieldDTO>().ReverseMap();
            CreateMap<StudentCustomField, StudentCustomFieldDTO>().ReverseMap();
            CreateMap<WorkList, WorkListDTO>().ReverseMap();
            CreateMap<WorkListNote, WorkListNoteDTO>().ReverseMap();
            CreateMap<Note, NoteDTO>().ReverseMap();
            CreateMap<File, FileDTO>().ReverseMap();
            CreateMap<History, HistoryDTO>().ReverseMap();
            CreateMap<HistoryWorkList, HistoryWorkListDTO>().ReverseMap();
            CreateMap<HistoryPerson, HistoryPersonDTO>().ReverseMap();
            CreateMap<HistoryStudent, HistoryStudentDTO>().ReverseMap();
            CreateMap<HistorySession, HistorySessionDTO>().ReverseMap();

            CreateMap<BulkStudentFile, StudentDTO>();
        }
    }
}