using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Application.Features.Admin.CustomField.Queries;
using Application.Features.Admin.Models;
using Application.Features.Admin.Student.Commands;
using AutoMapper;
using GraphQl.GraphQl.Features.Objects.Student.Inputs;
using GraphQl.GraphQl.Features.Objects.Student.Results;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace GraphQl.GraphQl.Features.Objects.Student;

[ExtendObjectType("Mutation"), Authorize]
public class StudentMutation
{
    public async Task<IStudentResult> AddStudent(
        AddStudentInput input,
        [Service] IMediator mediator,
        [Service] IMapper mapper)
    {
        if (input.CustomFields.Any())
        {
            var ids = await mediator.Send(new GetInvalidCustomFieldIds
            {
                LocationId = input.LocationId,
                Ids = input.CustomFields.Select(e => e.Id)
            });

            var invalidIds = ids as Guid[] ?? ids.ToArray();

            if (invalidIds.Any())
                return new InvalidCustomField
                {
                    Message = "Invalid custom fields",
                    Ids = invalidIds
                };
        }

        var student = new StudentDTO
        {
            LocationId = input.LocationId,
            FirstName = input.FirstName,
            MiddleName = input.MiddleName,
            LastName = input.LastName,
            Dob = input.Dob,
            Email = input.Email,
            GradeLevel = input.GradeLevel,
            StudentCustomFields = input.CustomFields.Any()
                ? input.CustomFields
                    .Select(field =>
                        new StudentCustomFieldDTO() { CustomFieldId = field.Id, Value = field.Value })
                    .ToList()
                : new List<StudentCustomFieldDTO>()
        };

        student = await mediator.Send(new AddStudent { StudentDTO = student, });

        return mapper.Map<Models.Student>(student);
    }
}
