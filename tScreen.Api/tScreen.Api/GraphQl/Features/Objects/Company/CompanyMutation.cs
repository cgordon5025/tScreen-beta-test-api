using System;
using System.Threading.Tasks;
using Application.Features.Admin.Company.Commands;
using Application.Features.Admin.Models;
using GraphQl.GraphQl.Features.Objects.Company.Inputs;
using GraphQl.GraphQl.Features.Objects.Company.Payloads;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;
using MediatR;

// ReSharper disable ClassNeverInstantiated.Global

namespace GraphQl.GraphQl.Features.Objects.Company;

[ExtendObjectType("Mutation"), Authorize]
public class CompanyMutation
{
    public async Task<AddCompanyPayload> AddCompany(
        AddCompanyInput input, 
        [Service] IMediator mediator)
    {
        var (type, name, description) = input;

        var company = new CompanyDTO
        {
            Type = type.ToString(),
            Name = name,
            Slug = name,
            Description = description
        };
            
        await mediator.Send(new AddCompany { CompanyDTO = company });

        Console.WriteLine($"Type: {type} | Name: {name} | Description: {description}");
        return new AddCompanyPayload(company);
    }

    public async Task<EditCompanyPayload> EditCompany(
        EditCompanyInput input, 
        [Service] Mediator mediator)
    {
        var (guid, type, name, description) = input;
            
        var company = new CompanyDTO
        {
            Id = guid,
            Type = type.ToString(),
            Name = name,
            Slug = name,
            Description = description
        };

        company = await mediator.Send(new EditCompany { CompanyDTO = company });

        return new EditCompanyPayload(company);
    }
}