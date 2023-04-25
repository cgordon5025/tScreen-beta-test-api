using HotChocolate.Execution.Configuration;
using HotChocolate.Types;
using Microsoft.Extensions.DependencyInjection;

namespace GraphQl.GraphQl.Features.Objects.CustomField.Results
{
    [UnionType("CustomFieldResult")]
    public interface ICustomField
    {
        
    }
    
    public class DuplicateField : ICustomField
    {
        public string? FieldName { get; set; }
    }
    
    public static class CustomFieldResultExtension
    {
        public static IRequestExecutorBuilder AddCustomFieldResultTypes(this IRequestExecutorBuilder builder)
        {
            builder
                .AddType<DuplicateField>();

            return builder;
        }
    }
}