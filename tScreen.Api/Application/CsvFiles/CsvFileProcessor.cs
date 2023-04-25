using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using FluentValidation;

namespace Application.CsvFiles;

public sealed class CsvFileProcessor<T>  : ICsvFileProcessor<T>
{
    private const string CsvFileExtension = ".csv";

    private readonly Stream? _stream;
    private readonly AbstractValidator<T> _validator;
    
    public CsvFileProcessor(string filePath, AbstractValidator<T> validator)
    {
        FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        _validator = validator;
        _stream = null;
    }

    public CsvFileProcessor(Stream stream, AbstractValidator<T> validator)
    {
        _stream = stream;
        _validator = validator;
    }

    public string? FilePath { get; }
    
    /// <summary>
    /// Parse and validated CSV file records into object collection.
    /// </summary>
    /// <typeparam name="TMap">Record map</typeparam>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<ICollection<CsvRecordResult<T>>> ParseAsync<TMap>() 
        where TMap : ClassMap<T>
    {
        if (_stream is null && FilePath is not null)
            _validateFileOrThrow();

        if (_validator is null)
            throw new Exception("CSV Validator is required but not found");

        using var streamReader = _stream is null ? new StreamReader(FilePath!) : new StreamReader(_stream);
        
        using var csvReader = new CsvReader(streamReader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            MissingFieldFound = null
        });
        
        csvReader.Context.RegisterClassMap<TMap>();
        var records = csvReader.GetRecordsAsync<T>();

        var i = 0;
        var outputRecords = new List<CsvRecordResult<T>>();
        await foreach (var record in records)
        {
            var validator = await _validator.ValidateAsync(record);

            var result = new CsvRecordResult<T>
            {
                RowIndex = i,
                Record = record,
                ValidationResult = validator
            };
            
            outputRecords.Add(result);
            i++;
        }

        return outputRecords;
    }

    private void _validateFileOrThrow()
    {
        if (!_isFile(FilePath) || !_isValidFileExtension(FilePath))
            throw new Exception("Cannot process non CSV files");
    }

    private static bool _isFile(string? path) => !string.IsNullOrWhiteSpace(path) && File.Exists(path);
    private static bool _isValidFileExtension(string? path) 
        => !string.IsNullOrWhiteSpace(path) && path.EndsWith(CsvFileExtension);
    
}