using System;
using Application.Common.Models;

namespace Application.Features.Admin.Models;

public class HistoryStudentDTO : BaseEntityDTO
{
    public Guid HistoryId { get; set; }
    public Guid StudentId { get; set; }

    public HistoryDTO? History { get; set; }
    public StudentDTO? Student { get; set; }
}