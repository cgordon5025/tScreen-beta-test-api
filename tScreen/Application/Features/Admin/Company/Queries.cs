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

namespace Application.Features.Admin.Company;

public class GetCompanyById : IRequest<CompanyDTO>
{
    public Guid CompanyId { get; set; }
    public bool ThrowWhenNull { get; set; } = true;

    internal sealed class GetCompanyByIdHandler : IRequestHandler<GetCompanyById, CompanyDTO>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public GetCompanyByIdHandler(IDbContextFactory<ApplicationDbContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<CompanyDTO> Handle(GetCompanyById request, CancellationToken cancellationToken)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await context.Company
                .TagWith(nameof(GetCompanyById))
                .TagWithCallSiteSafely()
                .Include(e => e.Locations)
                .Where(e => e.Id == request.CompanyId)
                .FirstOrDefaultAsync(cancellationToken);

            if (request.ThrowWhenNull && entity is null)
                throw new EntityNotFoundException(nameof(Company), request.CompanyId);

            return _mapper.Map<CompanyDTO>(entity);
        }
    }
}