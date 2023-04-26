using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.Admin.Models;
using AutoMapper;
using Core.Extensions;
using Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Admin.Person.Queries;

public class GetPersonByIdentityId : IRequest<PersonDTO>
{
    public Guid IdentityId { get; set; }

    internal sealed class GetPersonByIdentityIdHandler : IRequestHandler<GetPersonByIdentityId, PersonDTO>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public GetPersonByIdentityIdHandler(IDbContextFactory<ApplicationDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<PersonDTO> Handle(GetPersonByIdentityId request, CancellationToken cancellationToken)
        {
            if (Guid.Empty == request.IdentityId)
                throw new Exception("Empty GUID found");

            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await context.Person
                .TagWith(nameof(GetPersonByIdentityId))
                .TagWithCallSiteSafely()
                .Include(e => e.LocationPersons)
                .FirstOrDefaultAsync(e => e.Id == request.IdentityId, cancellationToken);

            return _mapper.Map<PersonDTO>(entity);
        }
    }
}