using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.Admin.Models;
using AutoMapper;
using Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Admin.Company.Commands
{
    public class EditCompany : IRequest<CompanyDTO>
    {
        public CompanyDTO? CompanyDTO { get; init; }
        
        internal sealed class EditCompanyHandler : IRequestHandler<EditCompany, CompanyDTO>
        {
            private readonly ApplicationDbContext _context;
            private readonly IMapper _mapper;
            
            public EditCompanyHandler(IDbContextFactory<ApplicationDbContext> contextFactory, IMapper mapper)
            {
                _context = contextFactory.CreateDbContext();
                _mapper = mapper;
            }
            
            public async Task<CompanyDTO> Handle(EditCompany request, CancellationToken cancellationToken)
            {
                if (request.CompanyDTO == null)
                    throw new ArgumentException(nameof(request.CompanyDTO));

                var entity = await _context.Company
                    .Where(e => e.Id == request.CompanyDTO.Id)
                    .Where(e => e.DeletedAt == null)
                    .FirstOrDefaultAsync(CancellationToken.None);

                if (entity == null)
                    throw new Exception("Entity not found");
                
                entity = _mapper.Map(request.CompanyDTO, entity);
                
                entity.UpdatedAt = DateTime.UtcNow;
                
                await _context.SaveChangesAsync(CancellationToken.None);

                return _mapper.Map<CompanyDTO>(entity);
            }
        }
    }
}