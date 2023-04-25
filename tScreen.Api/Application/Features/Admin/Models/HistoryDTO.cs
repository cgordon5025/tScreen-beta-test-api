using System;
using System.Collections.Generic;
using Application.Common.Models;

namespace Application.Features.Admin.Models;

public class HistoryDTO : BaseEntityDTO
{
    public Guid LocationId { get; set; }
    public Guid PersonId { get; set; }
    public string? Type { get; set; }
    public string? Data { get; set; }

    public ICollection<HistoryWorkListDTO> HistoryWorkLists { get; set; } = new HashSet<HistoryWorkListDTO>();
    public ICollection<HistoryPersonDTO> HistoryPersons { get; set; } = new HashSet<HistoryPersonDTO>();
    public ICollection<HistoryStudentDTO> HistoryStudents { get; set; } = new HashSet<HistoryStudentDTO>();
}