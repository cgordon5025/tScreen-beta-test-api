using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Admin.Models;
using Application.Features.App.Avatar.Commands;
using AutoMapper;
using Domain.Entities.App;
using GraphQl.GraphQl.Attributes;
using GraphQl.GraphQl.Features.Objects.Avatar.Inputs;
using GraphQl.GraphQl.Interfaces;
using GraphQl.GraphQl.Models;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace GraphQl.GraphQl.Features.Objects.Avatar;

[ExtendObjectType("Mutation"), Authorize]
public class AvatarMutation
{
    /// <summary>
    /// Create adventure avatar
    /// </summary>
    public async Task<IAvatarResult> AddAvatar(
        AddAvatarInput input,
        [ReferenceCode] string referenceCode,
        [Service] IValidateService validateService,
        [Service] IMediator mediator,
        [Service] IMapper mapper)
    {
        var errors = validateService.ValidateModel(input, new AddAvatarValidator());

        if (errors.Any())
            return new ValidationError
            {
                Message = "Cannot create avatar because input data is invalid",
                ReferenceCode = referenceCode,
                Errors = errors
            };

        var type = (input.BodyId != default || input.BodyColor != default || input.EyeColor != default
            || input.HairColor != default || input.ShirtColor != default || input.PantsColor != default
            || input.ShoesColor != default)
            ? AvatarTypes.UserDefined
            : AvatarTypes.SystemDefined;

        var avatarDTO = new AvatarDTO
        {
            StudentId = input.StudentId,
            Type = type,
            BodyId = input.BodyId,
            BodyColor = input.BodyColor,
            EyeColor = input.EyeColor,
            HairColor = input.HairColor,
            ShirtColor = input.ShirtColor,
            PantsColor = input.PantsColor,
            ShoesColor = input.ShoesColor,
            HelperId = input.HelperId
        };

        avatarDTO = await mediator.Send(new AddAvatar { AvatarDTO = avatarDTO });
        return mapper.Map<Models.Avatar>(avatarDTO);
    }
}