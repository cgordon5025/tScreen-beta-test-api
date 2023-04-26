using System.Collections.Generic;
using Application.Common.Models;

namespace GraphQl.GraphQl.Interfaces;

public interface IValidationError
{
    public string? Message { get; init; }
    public string? ReferenceCode { get; init; }
    public IEnumerable<ErrorDetail> Errors { get; init; }
}