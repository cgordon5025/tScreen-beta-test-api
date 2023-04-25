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

namespace Application.Features.App.File.Commands;

public class GetFileBySessionId : IRequest<FileDTO?>
{
    public Guid SessionId { get; init; }
    
    internal sealed class GetFileBySessionIdHandler : IRequestHandler<GetFileBySessionId, FileDTO?>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public GetFileBySessionIdHandler(IDbContextFactory<ApplicationDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }
        
        public async Task<FileDTO?> Handle(GetFileBySessionId request, CancellationToken cancellationToken)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var entity = await context.AppSessionFile
                .TagWith(nameof(GetFileBySessionId))
                .TagWithCallSiteSafely()
                .Where(e => e.SessionId == request.SessionId)
                .OrderByDescending(e => e.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            return _mapper.Map<FileDTO?>(entity);
        }
    }
}