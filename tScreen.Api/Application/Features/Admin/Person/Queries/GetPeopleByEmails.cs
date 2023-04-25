using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.Admin.Models;
using AutoMapper;
using Core.Extensions;
using Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Admin.Person.Queries;

public class GetPeopleByEmails : IRequest<Dictionary<string, PersonDTO>>
{
    public Guid CompanyId { get; set; }
    public IEnumerable<string> Emails { get; init; }
    
    internal sealed class GetPeopleByEmailsHandler : 
        IRequestHandler<GetPeopleByEmails, Dictionary<string, PersonDTO>>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly ILogger<GetPeopleByEmails> _logger;
        private readonly IMapper _mapper;

        public GetPeopleByEmailsHandler(
            IDbContextFactory<ApplicationDbContext> contextFactory, 
            ILogger<GetPeopleByEmails> logger,
            IMapper mapper
        )
        {
            _contextFactory = contextFactory;
            _logger = logger;
            _mapper = mapper;
        }
        
        public async Task<Dictionary<string, PersonDTO>> Handle(
            GetPeopleByEmails request, CancellationToken cancellationToken)
        {
            if (!request.Emails.Any())
            {
                _logger.LogInformation("No emails not found, no operation performed");
                return new Dictionary<string, PersonDTO>();
            }
            
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

            var normalizeEmails = request.Emails
                .Select(e => e.ToUpper(CultureInfo.InvariantCulture));

            var collection = await context.Person
                .TagWith($"{nameof(GetPeopleByEmails)}.{nameof(GetPeopleByEmailsHandler)}")
                .TagWithCallSiteSafely()
                .Include(e => e.User)
                .Where(e => e.User != null && 
                            normalizeEmails.Contains(e.User.NormalizedEmail) && e.CompanyId == request.CompanyId)
                .ToDictionaryAsync(
                    e => e.User!.Email, 
                    e => _mapper.Map<PersonDTO>(e), cancellationToken);

            return collection;
        }
    }
}