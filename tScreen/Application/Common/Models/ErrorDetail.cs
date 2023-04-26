using System.Collections.Generic;

namespace Application.Common.Models;

public class ErrorDetail
{
    public string FieldName { get; set; } = null!;
    public IEnumerable<string> Messages { get; set; } = new List<string>();
}