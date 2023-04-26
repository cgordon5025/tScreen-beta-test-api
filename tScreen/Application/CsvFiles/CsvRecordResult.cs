using FluentValidation.Results;
using System.ComponentModel.DataAnnotations;

namespace Application.CsvFiles;

public class CsvRecordResult<T>
{
    public int RowIndex { get; set; }
    public T Record { get; set; } = default!;
    public ValidationResult ValidationResult { get; set; } = default!;
}