using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Data;
using GraphQl.GraphQl.DataLoaders;
using GreenDonut;
using HotChocolate;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace GraphQl.GraphQl.Features.Objects.StudentCustomField;


public class StudentCustomFieldType : ObjectType<Models.StudentCustomField>
{
    protected override void Configure(IObjectTypeDescriptor<Models.StudentCustomField> descriptor)
    {
        descriptor.Field(e => e.CustomField)
            .UseDbContext<ApplicationDbContext>()
            .ResolveWith<StudentCustomFieldResolver>(r =>
                r.GetCustomFields(default!, default!, default!, default));
    }

    private class StudentCustomFieldResolver
    {
        public async Task<IEnumerable<Models.CustomField>> GetCustomFields(
            [Parent] Models.StudentCustomField studentCustomField,
            [ScopedService] ApplicationDbContext context,
            CustomFieldByIdDataLoader dataLoader,
            CancellationToken cancellationToken)
        {
            var ids = await context.StudentCustomField
                .Include(e => e.CustomField)
                .Where(e => e.CustomField.Id == studentCustomField.CustomField.Id)
                .Select(e => e.CustomFieldId)
                .ToListAsync(cancellationToken);

            return await dataLoader.LoadAsync(ids, cancellationToken);
        }
    }
}