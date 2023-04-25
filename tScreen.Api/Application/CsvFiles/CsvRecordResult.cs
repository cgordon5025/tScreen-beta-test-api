using FluentValidation.Results;

namespace Application.CsvFiles;

public class CsvRecordResult<T>
{
    public int RowIndex { get; set; }
    public T Record { get; set; } = default!;
    public ValidationResult ValidationResult { get; set; } = default!;
}