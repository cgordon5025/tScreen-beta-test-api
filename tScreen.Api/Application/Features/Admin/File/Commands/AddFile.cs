using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Admin.Models;
using AutoMapper;
using Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Admin.File.Commands;

public class AddFile : IRequest<FileDTO>
{
    public FileDTO FileDTO { get; set; }

    internal sealed class AddFileHandler : IRequestHandler<AddFile, FileDTO>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IMapper _mapper;
        private readonly IBlobStorage _blobStorage;
            
        public AddFileHandler(IDbContextFactory<ApplicationDbContext> contextFactoryFactory, IMapper mapper, IBlobStorage blobStorage)
        {
            _contextFactory = contextFactoryFactory;
            _mapper = mapper;
            _blobStorage = blobStorage;
        }
        
        public async Task<FileDTO> Handle(AddFile request, CancellationToken cancellationToken)
        { 
            if (request.FileDTO.File == null)
                throw new ArgumentNullException(nameof(request.FileDTO.File));
            
            await using var context = await _contextFactory.CreateDbContextAsync(CancellationToken.None);

            var entity = _mapper.Map<Domain.Entities.File>(request.FileDTO);
            context.File.Add(entity);

            _blobStorage.SetStorageAccount(BlobStorageType.Client);
            await _blobStorage.UploadBlobWithNewNameAsync(request.FileDTO.File.OpenReadStream(), 
                request.FileDTO.StorageContainer!);

            entity.StorageAccount = _blobStorage.StorageAccountName;
            entity.BlobName = _blobStorage.LastUsedFileName;
            
            await context.SaveChangesAsync(CancellationToken.None);

            return _mapper.Map<FileDTO>(entity);
        }
    }
}