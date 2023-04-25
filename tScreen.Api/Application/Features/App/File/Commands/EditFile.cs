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

namespace Application.Features.App.File.Commands;

public class EditFile : IRequest<FileDTO>
{
    public FileDTO FileDTO { get; init; }
    
    internal sealed class EditFileHandler : IRequestHandler<EditFile, FileDTO>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public EditFileHandler(IDbContextFactory<ApplicationDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }
        
        public async Task<FileDTO> Handle(EditFile request, CancellationToken cancellationToken)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(CancellationToken.None);
            
            var entity = await context.AppSessionFile
                .TagWith(nameof(EditFile))
                .TagWithCallSiteSafely()
                .Where(e => e.Id == request.FileDTO.Id)
                .FirstOrDefaultAsync(CancellationToken.None);

            if (entity is null)
                throw new EntityNotFoundException("File", request.FileDTO.Id);
            
            entity = _mapper.Map(request.FileDTO, entity);
            entity.UpdatedAt = DateTime.UtcNow;
            
            await context.SaveChangesAsync(CancellationToken.None);
            
            return _mapper.Map<FileDTO>(entity);
;        }
    }
}