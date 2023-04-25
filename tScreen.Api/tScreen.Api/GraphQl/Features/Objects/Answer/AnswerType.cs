using System.Threading;
using System.Threading.Tasks;
using Data;
using GraphQl.GraphQl.DataLoaders;
using HotChocolate;
using HotChocolate.Types;

// ReSharper disable ClassNeverInstantiated.Local

namespace GraphQl.GraphQl.Features.Objects.Answer;

public class AnswerType : ObjectType<Models.Answer>
{
    protected override void Configure(IObjectTypeDescriptor<Models.Answer> descriptor)
    {
        descriptor.Field(e => e.Session)
            .UseDbContext<ApplicationDbContext>()
            .ResolveWith<AnswerResolvers>(r
                => r.GetSessionAsync(default!, default!, default))
            .Name("session");

        descriptor.Field(e => e.Question)
            .UseDbContext<ApplicationDbContext>()
            .ResolveWith<AnswerResolvers>(r
                => r.GetQuestionAsync(default!, default!, default))
            .Name("question");
    }

    private class AnswerResolvers
    {
        public async Task<Models.Session?> GetSessionAsync(
            [Parent] Models.Answer answer,
            SessionDataByIdDataLoader dataLoader,
            CancellationToken cancellationToken)
            => await dataLoader.LoadAsync(answer.SessionId, cancellationToken);
        
        public async Task<Models.Question?> GetQuestionAsync(
            [Parent] Models.Answer answer,
            QuestionByIdDataLoader dataLoader,
            CancellationToken cancellationToken)
            => await dataLoader.LoadAsync(answer.QuestionId, cancellationToken);
    }
}