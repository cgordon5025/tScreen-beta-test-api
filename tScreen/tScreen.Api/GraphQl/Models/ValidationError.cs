using System.Collections.Generic;
using Application.Common.Models;
using GraphQl.GraphQl.Interfaces;

namespace GraphQl.GraphQl.Models;

public class ValidationError : IValidationError, ISessionResult, IWorkListResult, IAvatarResult, IAnswerResult
{
    public string? Message { get; init; }
    public string? ReferenceCode { get; init; }
    public IEnumerable<ErrorDetail> Errors { get; init; } = new List<ErrorDetail>();
}