using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.Admin.Models;
using AutoMapper;
using Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Admin.CustomField.Commands
{
    public class AddCustomField : IRequest<Guid>
    {
        public CustomFieldDTO? CustomFieldDTO { get; init; }
        
        internal sealed class AddCustomFieldHandler : IRequestHandler<AddCustomField, Guid>
        {
            private readonly ApplicationDbContext _context;
            private readonly IMapper _mapper;
            
            public AddCustomFieldHandler(IDbContextFactory<ApplicationDbContext> context, IMapper mapper)
            {
                _context = context.CreateDbContext();
                _mapper = mapper;
            }
            
            public async Task<Guid> Handle(AddCustomField request, CancellationToken cancellationToken)
            {
                var entity = _mapper.Map<Domain.Entities.CustomField>(request.CustomFieldDTO);
                _context.CustomField.Add(entity);
                
                await _context.SaveChangesAsync(CancellationToken.None);
                
                return entity.Id;
            }
        }
    }
}