using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.Admin.Models;
using AutoMapper;
using Data;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Admin.Company.Commands
{
    public class AddCompany : IRequest<Guid>
    {
        public CompanyDTO? CompanyDTO { get; init; }
        
        internal sealed class AddCompanyHandler : IRequestHandler<AddCompany, Guid>
        {
            private readonly ApplicationDbContext _context;
            private readonly IMapper _mapper;
            
            public AddCompanyHandler(IDbContextFactory<ApplicationDbContext> contextFactory, IMapper mapper)
            {
                _context = contextFactory.CreateDbContext();
                _mapper = mapper;
            }
            
            public async Task<Guid> Handle(AddCompany request, CancellationToken cancellationToken)
            {
                await using var transaction = await _context.Database.BeginTransactionAsync(CancellationToken.None);
                
                var company = _mapper.Map<Domain.Entities.Company>(request.CompanyDTO);

                var location = new Location
                {
                    Name = "Default",
                    Description = "Default location",
                    Country = "USA",
                    CreatedAt = DateTime.UtcNow
                };
                
                company.Locations.Add(location);
                
                _context.Company.Add(company);
                await _context.SaveChangesAsync(CancellationToken.None);
                
                company.Persons.Add(new Domain.Entities.Person
                {
                    // IdentityId = "Test_" + Guid.NewGuid(),
                    // IdentityType = "TestAccount",
                    LocationPersons = new List<LocationPerson>
                    {
                        new() { LocationId = location.Id }
                    }
                });
                
                await _context.SaveChangesAsync(CancellationToken.None);

                await transaction.CommitAsync(CancellationToken.None);

                return company.Id;
            }
        }
    }
}