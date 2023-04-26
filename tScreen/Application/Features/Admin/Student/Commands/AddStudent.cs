using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Events;
using Application.Features.Admin.Models;
using AutoMapper;
using Data;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Admin.Student.Commands
{
    public class AddStudent : IRequest<StudentDTO>
    {
        public Guid CompanyId { get; init; }
        public StudentDTO? StudentDTO { get; init; }
        public IEnumerable<Guid> PersonIds { get; init; } = Enumerable.Empty<Guid>();
        public CustomFieldValueDTO? CustomFieldValueDTO { get; init; }

        internal sealed class AddStudentHandler : IRequestHandler<AddStudent, StudentDTO>
        {
            private readonly ApplicationDbContext _context;
            private readonly IMediator _mediator;
            private readonly IMapper _mapper;

            public AddStudentHandler(IDbContextFactory<ApplicationDbContext> contextFactory, IMediator mediator,
                IMapper mapper)
            {
                _context = contextFactory.CreateDbContext();
                _mediator = mediator;
                _mapper = mapper;
            }

            public async Task<StudentDTO> Handle(AddStudent request, CancellationToken cancellationToken)
            {
                var student = _mapper.Map<Domain.Entities.Student>(request.StudentDTO);

                var entity = await _context.Student
                    .Where(e => e.LocationId == request.StudentDTO!.LocationId)
                    .Where(e => !string.IsNullOrWhiteSpace(e.Email) && e.Email == request.StudentDTO!.Email)
                    .FirstOrDefaultAsync(CancellationToken.None);

                if (entity is not null)
                    throw new EntityDuplicateException(nameof(Domain.Entities.Student),
                        new { request.StudentDTO?.LocationId, request.StudentDTO?.Email }, entity.CreatedAt);

                var persons = await _context.LocationPerson
                    .Include(e => e.Location)
                    .Where(e => e.Location!.CompanyId == request.CompanyId)
                    .ToDictionaryAsync(e => e.Id, cancellationToken);

                // Add any person/player associations as long as the link doesn't already exist
                foreach (var personId in request.PersonIds)
                    if (!persons.ContainsKey(personId))
                        student.PersonStudents.Add(new PersonStudent { PersonId = personId });

                if (request.CustomFieldValueDTO?.CustomFieldValueOne is not null ||
                    request.CustomFieldValueDTO?.CustomFieldValueTwo is not null)
                {
                    var customFields = await _context.CustomField
                        .Where(e => e.LocationId == request.StudentDTO!.LocationId)
                        .OrderBy(e => e.Position)
                        .ToListAsync(CancellationToken.None);

                    var customFieldValues = new[]
                    {
                        request.CustomFieldValueDTO.CustomFieldValueOne,
                        request.CustomFieldValueDTO.CustomFieldValueTwo
                    };

                    var index = 0;
                    foreach (var customField in customFields)
                    {
                        student.StudentCustomFields.Add(new StudentCustomField
                        {
                            CustomFieldId = customField.Id,
                            Value = customFieldValues[index++]
                        });
                    }
                }

                _context.Student.Add(student);
                await _context.SaveChangesAsync(CancellationToken.None);

                await _mediator.Publish(new StudentCreatedEvent(student), CancellationToken.None);

                return _mapper.Map<StudentDTO>(student);
            }
        }
    }
}