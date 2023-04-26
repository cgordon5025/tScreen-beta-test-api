using System;
using Application.Common.Models;

namespace Application.Features.Admin.Models;

public class LocationPersonDTO : BaseEntityDTO
{
    public Guid LocationId { get; set; }
    public Guid PersonId { get; set; }
    public string? Type { get; set; }

    public LocationDTO? Location { get; set; }
    public PersonDTO? Person { get; set; }
}