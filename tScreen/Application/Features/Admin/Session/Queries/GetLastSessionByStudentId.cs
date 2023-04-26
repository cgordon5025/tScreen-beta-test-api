using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Features.Admin.Models;
using AutoMapper;
using Core.Extensions;
using Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Admin.Session.Queries;

public class GetLastSessionByStudentId : IRequest<SessionDTO>
{
    public Guid PersonId { get; init; }
    public bool ThrowIfNotFound { get; set; } = false;

    internal sealed class GetLastSessionByStudentIdHandled : IRequestHandler<GetLastSessionByStudentId, SessionDTO>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public GetLastSessionByStudentIdHandled(IDbContextFactory<ApplicationDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<SessionDTO> Handle(GetLastSessionByStudentId request, CancellationToken cancellationToken)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

            var entity = context.AppSessions
                .TagWith(nameof(GetLastSessionByStudentId))
                .TagWithCallSiteSafely()
                .Where(e => e.PersonId == request.PersonId)
                .MaxBy(e => e.CreatedAt);

            if (request.ThrowIfNotFound && entity is null)
                throw new EntityNotFoundException(nameof(Session), nameof(request.PersonId), request.PersonId);

            return _mapper.Map<SessionDTO>(entity);
        }
    }
}