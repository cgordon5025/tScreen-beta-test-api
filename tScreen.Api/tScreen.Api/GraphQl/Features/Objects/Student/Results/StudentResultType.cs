using System;
using HotChocolate.Execution.Configuration;
using HotChocolate.Types;
using Microsoft.Extensions.DependencyInjection;

namespace GraphQl.GraphQl.Features.Objects.Student.Results
{
    [UnionType("StudentResult")]
    public interface IStudentResult 
    {}

    // public class StudentPayload : IStudentResult
    // { }

    public class InvalidCustomField : IStudentResult
    {
        public string? Message { get; set; }
        public Guid[]? Ids { get; set; }
    }

    public static class StudentResultExtension
    {
        public static IRequestExecutorBuilder AddStudentResultTypes(this IRequestExecutorBuilder builder)
        {
            builder
                .AddType<InvalidCustomField>();

            return builder;
        }
    }
}