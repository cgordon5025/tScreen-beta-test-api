using System.Reflection;
using System.Threading.Tasks;
using Application.Features.Admin.CustomField.Commands;
using Application.Features.Admin.Models;
using AutoMapper;
using GraphQl.GraphQl.Features.Objects.CustomField.Inputs;
using GraphQl.GraphQl.Features.Objects.CustomField.Results;
using HotChocolate;
using HotChocolate.Types;
using MediatR;

namespace GraphQl.GraphQl.Features.Objects.CustomField
{
    [ExtendObjectType("Mutation")]
    public class CustomFieldMutation
    {
        public async Task<ICustomField> AddCustomElement
            (AddCustomFieldInput input, [Service] IMediator mediator, [Service] IMapper mapper)
        {
            var customField = new CustomFieldDTO
            {
                LocationId = input.LocationId,
                Type = input.Type.ToString(),
                Position = input.Position,
                Name = input.Name,
                Description = input.Description,
                PlaceHolder = input.Placeholder,
                DefaultValue = input.DefaultValue,
                ValidationRule = input.ValidationRule
            };

            var id = await mediator.Send(new AddCustomField { CustomFieldDTO = customField });
            customField.Id = id;

            return mapper.Map<Models.CustomField>(customField);
        }
    }
}