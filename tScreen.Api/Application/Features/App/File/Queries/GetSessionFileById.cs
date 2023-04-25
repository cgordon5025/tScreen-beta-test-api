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

namespace Application.Features.App.File.Queries;

public class GetSessionFileById : IRequest<FileDTO>
{
    public Guid FileId { get; init; }
    public bool ShouldThrow { get; init; } = false;

    internal sealed class GetFileByIdHandler : IRequestHandler<GetSessionFileById, FileDTO>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public GetFileByIdHandler(IDbContextFactory<ApplicationDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }
        
        public async Task<FileDTO> Handle(GetSessionFileById request, CancellationToken cancellationToken)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await context.AppSessionFile
                .TagWith(nameof(GetSessionFileById))
                .TagWithCallSiteSafely()
                .Include(e => e.Session)
                .Where(e => e.Id == request.FileId)
                .FirstOrDefaultAsync(cancellationToken);

            if (entity is null && request.ShouldThrow)
                throw new EntityNotFoundException(nameof(Domain.Entities.App.File), request.FileId);
            
            return _mapper.Map<FileDTO>(entity);
        }
    }
}