using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Admin.Models;
using Application.Features.App.Answer.Commands;
using AutoMapper;
using GraphQl.GraphQl.Attributes;
using GraphQl.GraphQl.Features.Objects.Answer.inputs;
using GraphQl.GraphQl.Interfaces;
using GraphQl.GraphQl.Models;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;
using MediatR;

namespace GraphQl.GraphQl.Features.Objects.Answer;

[ExtendObjectType("Mutation"), Authorize]
public class AnswerMutation
{
    /// <summary>
    /// Add answer to Question
    /// </summary>
    public async Task<IAnswerResult> AddAnswer(
        AddAnswerInput input,
        [ReferenceCode] string referenceCode,
        [Service] IValidateService validateService,
        [Service] IMediator mediator,
        [Service] IMapper mapper)
    {
        var errors = validateService.ValidateModel(input, new AddAnswerValidator());

        if (errors.Any())
            return new ValidationError
            {
                Message = "Cannot add answer because input data is invalid",
                ReferenceCode = referenceCode,
                Errors = errors
            };
        
        var answerDTO = new AnswerDTO
        {
            SessionId = input.SessionId,
            QuestionId = input.QuestionId,
            Data = input.Data
        };
        
        answerDTO = await mediator.Send(new AddAnswer { AnswerDTO = answerDTO });

        return mapper.Map<Models.Answer>(answerDTO);
    }
}