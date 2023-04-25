using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.Admin.Models;
using AutoMapper;
using Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Admin.Person.Commands;

public class AddPerson : IRequest<PersonDTO>
{
    public PersonDTO PersonDTO { get; set; } = null!;
    
    internal sealed class AddPersonHandler : IRequestHandler<AddPerson, PersonDTO>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IMapper _mapper;
        
        public AddPersonHandler(IDbContextFactory<ApplicationDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }
        
        public async Task<PersonDTO> Handle(AddPerson request, CancellationToken cancellationToken)
        {
            if (request.PersonDTO is null)
                throw new NullReferenceException(nameof(request.PersonDTO));

            await using var context = await _contextFactory.CreateDbContextAsync(CancellationToken.None);

            var entity = _mapper.Map<Domain.Entities.Person>(request.PersonDTO);
            await context.Person.AddAsync(entity, CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);
                
            return _mapper.Map<PersonDTO>(entity);
;        }
    }
}