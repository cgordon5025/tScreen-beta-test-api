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

public class GetSessionById : IRequest<SessionDTO?>
{
    public Guid SessionId { get; init; }
    public bool ShouldThrow { get; init; } = false;
    public bool IncludeStudent { get; init; } = false;

    internal sealed class GetSessionByIdHandler : IRequestHandler<GetSessionById, SessionDTO?>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public GetSessionByIdHandler(IDbContextFactory<ApplicationDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<SessionDTO?> Handle(GetSessionById request, CancellationToken cancellationToken)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

            var query = context.AppSessions
                .TagWith(nameof(GetSessionById))
                .TagWithCallSiteSafely()
                .Where(e => e.Id == request.SessionId);

            if (request.IncludeStudent)
                query = query.Include(e => e.Student);

            var entity = await query.FirstOrDefaultAsync(cancellationToken);

            if (entity is null && request.ShouldThrow)
                throw new EntityNotFoundException(nameof(Domain.Entities.App.Session), request.SessionId);

            return _mapper.Map<SessionDTO>(entity);
        }
    }
}