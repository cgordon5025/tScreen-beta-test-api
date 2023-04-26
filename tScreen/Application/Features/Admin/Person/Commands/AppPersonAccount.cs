using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Application.Events;
using Application.Features.Admin.Models;
using AutoMapper;
using Core;
using Data;
using Domain.Entities;
using Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Admin.Person.Commands;

public class AppPersonAccount : IRequest<Unit>
{
    public Guid CompanyId { get; init; }
    public Guid LocationId { get; init; }

    public PersonDTO PersonDTO { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string? Password { get; init; }
    public IEnumerable<string>? Roles { get; init; }

    internal sealed class AddPersonAccountHandler : IRequestHandler<AppPersonAccount, Unit>
    {
        private readonly ILogger<AddPersonAccountHandler> _logger;
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public AddPersonAccountHandler(
            ILogger<AddPersonAccountHandler> logger,
            IDbContextFactory<ApplicationDbContext> contextFactory,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IMediator mediator,
            IMapper mapper
        )
        {
            _logger = logger;
            _contextFactory = contextFactory;
            _userManager = userManager;
            _roleManager = roleManager;
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(AppPersonAccount request, CancellationToken cancellationToken)
        {
            if (request?.PersonDTO is null)
                throw new NullReferenceException(nameof(request.PersonDTO));

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new NullReferenceException(nameof(request.Email));

            await using var context = await _contextFactory.CreateDbContextAsync(CancellationToken.None);
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                var rolesList = request?.Roles?.ToList() ?? Enumerable.Empty<string>().ToList();

                // Check if the roles provided do exist, if not exit early
                if (rolesList.Any())
                    foreach (var role in rolesList)
                        if (!await _roleManager.RoleExistsAsync(role))
                            throw new Exception($"Role {role} does not exist");

                var user = new User
                {
                    CompanyId = request!.CompanyId,
                    Email = request.Email,
                    NormalizedEmail = request.Email.ToUpper(),
                    UserName = request.Email,
                    NormalizedUserName = request.Email.ToUpper(),
                    LockoutEnabled = false,
                    EmailConfirmed = true
                };

                var password = request.Password ?? Generate.CryptoRandomPassword(10);
                var result = await _userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    var firstError = result.Errors.FirstOrDefault();
                    throw new Exception($"Unable to create new account. Code: {firstError?.Code ?? "Unknown"}  Error: {firstError?.Description ?? "Unknown"}");
                }

                request.PersonDTO.CompanyId = request.CompanyId;
                request.PersonDTO.Id = user.Id;

                // If roles are present bind to user
                // Note: intentionally separate from validation/exit early logic above
                if (rolesList.Any())
                    await _userManager.AddToRolesAsync(user, rolesList);

                // var claims = roleClaims.Select(roleClaim => 
                //     new Claim(roleClaim.ClaimType, roleClaim.ClaimValue));
                // await _userManager.AddClaimsAsync(user, claims);

                var entity = _mapper.Map<Domain.Entities.Person>(request.PersonDTO);
                entity.LocationPersons.Add(new LocationPerson
                {
                    PersonId = user.Id,
                    LocationId = request.LocationId,
                    Type = LocationTypes.Default
                });

                await context.Person.AddAsync(entity, CancellationToken.None);
                await context.SaveChangesAsync(CancellationToken.None);

                scope.Complete();

                _logger.LogInformation("User account {Email} successfully created", request.Email);

                await _mediator.Publish(
                    new PersonCreatedEvent(entity, request.Email, password),
                    CancellationToken.None);
            }
            catch (Exception e)
            {
                scope.Dispose();
                throw;
            }

            return Unit.Value;
        }
    }
}