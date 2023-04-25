using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.Admin.Models;
using AutoMapper;
using Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.App.File.Commands;

public class AddFile : IRequest<FileDTO>
{
    public Guid SessionId { get; init; }
    public FileDTO FileDTO { get; init; }

    internal sealed class AddFileHandler : IRequestHandler<AddFile, FileDTO>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;
        private readonly IMapper _mapper;

        public AddFileHandler(IDbContextFactory<ApplicationDbContext> factory, IMapper mapper)
        {
            _factory = factory;
            _mapper = mapper;
        }
        
        public async Task<FileDTO> Handle(AddFile request, CancellationToken cancellationToken)
        {
            if (request.FileDTO == null)
                throw new NullReferenceException(nameof(request.FileDTO));
            
            await using var context = await _factory.CreateDbContextAsync(CancellationToken.None);
            
            var entity = _mapper.Map<Domain.Entities.App.File>(request.FileDTO);
            entity.SessionId = request.SessionId;
            
            await context.AppSessionFile.AddAsync(entity, CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);

            return _mapper.Map<FileDTO>(entity);
        }
    }
}