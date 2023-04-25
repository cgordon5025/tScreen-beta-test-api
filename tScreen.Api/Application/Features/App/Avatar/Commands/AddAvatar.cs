using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.Admin.Models;
using AutoMapper;
using Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.App.Avatar.Commands;

public class AddAvatar : IRequest<AvatarDTO>
{
    public AvatarDTO? AvatarDTO { get; set; }
    
    internal sealed class AddAvatarHandler : IRequestHandler<AddAvatar, AvatarDTO>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public AddAvatarHandler(IDbContextFactory<ApplicationDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }
        
        public async Task<AvatarDTO> Handle(AddAvatar request, CancellationToken cancellationToken)
        {
            if (request.AvatarDTO is null)
                throw new NullReferenceException(nameof(request.AvatarDTO));

            await using var context = await _contextFactory.CreateDbContextAsync(CancellationToken.None);

            var entity = _mapper.Map<Domain.Entities.App.Avatar>(request.AvatarDTO);
            await context.AddAsync(entity, CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);

            return _mapper.Map<AvatarDTO>(entity);
        }
    }
}