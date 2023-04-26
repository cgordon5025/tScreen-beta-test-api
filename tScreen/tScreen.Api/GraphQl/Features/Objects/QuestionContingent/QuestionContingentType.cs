using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Data;
using GraphQl.GraphQl.DataLoaders;
using HotChocolate;
using HotChocolate.Types;

// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable ClassNeverInstantiated.Global

namespace GraphQl.GraphQl.Features.Objects.QuestionContingent;

public class QuestionContingentType : ObjectType<Models.QuestionContingent>
{
    protected override void Configure(IObjectTypeDescriptor<Models.QuestionContingent> descriptor)
    {
        descriptor.Field(e => e.QuestionParent)
            .UseDbContext<ApplicationDbContext>()
            .ResolveWith<QuestionContingentResolvers>(r =>
                r.GetQuestionParent(default!, default!, default))
            .Name("questionParent");

        descriptor.Field(e => e.Question)
            .UseDbContext<ApplicationDbContext>()
            .ResolveWith<QuestionContingentResolvers>(r =>
                r.GetQuestion(default!, default!, default))
            .Name("question");
    }

    private class QuestionContingentResolvers
    {
        public async Task<Models.Question> GetQuestionParent(
            [Parent] Models.QuestionContingent questionContingent,
            QuestionByIdDataLoader dataLoader,
            CancellationToken cancellationToken)
            => await dataLoader.LoadAsync(questionContingent.ParentId, cancellationToken);

        public async Task<Models.Question> GetQuestion(
            [Parent] Models.QuestionContingent questionContingent,
            QuestionByIdDataLoader dataLoader,
            CancellationToken cancellationToken)
            => await dataLoader.LoadAsync(questionContingent.QuestionId, cancellationToken);
    }
}