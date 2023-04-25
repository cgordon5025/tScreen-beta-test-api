using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Interfaces;
using Application.CsvFiles;
using Application.CsvFiles.Student;
using Application.CsvFiles.User;
using Application.Features.Admin.Company;
using Application.Features.Admin.File.Commands;
using Application.Features.Admin.Models;
using Application.Features.Admin.Person.Commands;
using Application.Features.Admin.Person.Queries;
using Application.Features.Admin.Student.Commands;
using Application.Features.App.File.Queries;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GraphQl.Controllers;

[Route("api/file")]
[ApiController]
public class FileController : Controller
{
    private readonly IMediator _mediator;
    private readonly IBlobStorage _blobStorage;
    private readonly IMapper _mapper;
    private readonly ILogger<FileController> _logger;

    public FileController(IMediator mediator, IBlobStorage blobStorage, IMapper mapper, ILogger<FileController> logger)
    {
        _mediator = mediator;
        _blobStorage = blobStorage;
        _mapper = mapper;
        _logger = logger;
    }
    
    [HttpGet("download/client/{type:alpha}/{fileId::guid}")]
    public async Task<IActionResult> DownloadClientFile(string type, Guid fileId)
    {
        if (type != StorageContainers.ReportsStorage.Name) return BadRequest();

        var fileDTO = await _mediator.Send(new GetSessionFileById { FileId = fileId, ShouldThrow = true });
        
        _blobStorage.SetStorageAccount(BlobStorageType.Client);

        var normalizedLocationId = fileDTO.Session!.LocationId.ToString().ToLowerInvariant();
        var absolutePath = $"{fileDTO.StorageContainer}/{normalizedLocationId}";
        var response = await _blobStorage
            .DownloadAsync(absolutePath, fileDTO.BlobName!);

        var contentType = fileDTO.MimeType ?? response.Value.ContentType;
        return File(response.Value.Content, contentType);
    }
    
    [HttpPost("client")]
    public async Task UploadFileToAdmin([FromForm] UploadClientFile model)
    {
        _blobStorage.SetStorageAccount(BlobStorageType.Client);

        if (model.File != null)
            await _mediator.Send(new AddFile
            {
                FileDTO = new FileDTO
                {
                    LocationId = model.LocationId,
                    Category = "test",
                    MimeType = model.File.ContentType,
                    FileName = model.File.Name,
                    FileSize = (int) model.File.Length,
                    DisplayName = model.File.Name,
                    StorageContainer = "test",
                    Description = "test file",
                    File = model.File,
                }
            });
    }

    [HttpPost("core")]
    public async Task UploadFileToCore([FromForm] UploadClientFile model)
    {
        _blobStorage.SetStorageAccount(BlobStorageType.Core);
        if (model.File != null)
            await _mediator.Send(new AddFile
            {
                FileDTO = new FileDTO
                {
                    LocationId = model.LocationId,
                    Category = "test",
                    MimeType = model.File.ContentType,
                    FileName = model.File.Name,
                    FileSize = (int) model.File.Length,
                    DisplayName = model.File.Name,
                    StorageContainer = "test",
                    Description = "test file",
                    File = model.File,
                }
            });
    }

    [HttpPost("bulk/person")]
    public async Task<IActionResult> UploadBulkPersonFile([FromForm] Guid companyId, 
        [FromForm] Guid locationId, [FromForm] IFormFile file)
    {
        if (Request.ContentType != null && !Request.ContentType.Contains("multipart/form-data"))
            return StatusCode(StatusCodes.Status415UnsupportedMediaType);
        
        var companyDTO = await _mediator.Send(new GetCompanyById { CompanyId = companyId });
        
        if (companyDTO is null)
            return BadRequest($"Company {companyId} not found");
        
        var csvFileProcessor = new CsvFileProcessor<BulkUserFile>(file.OpenReadStream(), new BulkUserFileValidator());
        var records = await csvFileProcessor.ParseAsync<BulkUserFileMap>();
        
        // All records must pass validation before any record is insert into the DB
        // if validation errors are found, return all records.
        if (records.Any(record => !record.ValidationResult.IsValid))
            return BadRequest(records);

        var problemRecords = new List<ProblemRecord<BulkUserFile>>();
        foreach (var person in records)
        {
            var personDTO = _mapper.Map<PersonDTO>(person.Record);

            try
            {
                await _mediator.Send(new AppPersonAccount
                {
                    CompanyId = companyId,
                    LocationId = locationId,
                    PersonDTO = personDTO,
                    Email = person.Record.Email,
                    Password = person.Record.Email.Contains("biguat.futuresthrive.com") ? "Biguat1$" : null,
                    Roles = new[] { person.Record.UserRole }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception: {Message}", ex.Message);
                problemRecords.Add(new ProblemRecord<BulkUserFile>
                {
                    Record = person.Record,
                    Message = ex.Message
                });
            }
        }
        
        return Ok(new
        {
            CsvFile = records,
            FailedToProccessRecord = problemRecords
        });
    }

    [HttpPost("bulk/player")]
    public async Task<IActionResult> UploadBulkPlayerFile([FromForm] Guid companyId, 
        [FromForm] Guid locationId, [FromForm] IFormFile file)
    {
        if (Request.ContentType != null && !Request.ContentType.Contains("multipart/form-data"))
            return StatusCode(StatusCodes.Status415UnsupportedMediaType);
        
        var csvFileProcessor = new CsvFileProcessor<BulkStudentFile>(file.OpenReadStream(), new BulkStudentFileValidator());
        var records = await csvFileProcessor.ParseAsync<BulkStudentFileMap>();
        
        // All records must pass validation before any record is insert into the DB
        // if validation errors are found, return all records.
        if (records.Any(record => !record.ValidationResult.IsValid))
            return BadRequest(records);

        // Need to get the default location ID, a requirement for the student table
        var companyDTO = await _mediator.Send(new GetCompanyById { CompanyId = companyId });

        if (companyDTO is null)
            return BadRequest($"Company {companyId} not found");
        
        var locationDTO = companyDTO.Locations.FirstOrDefault(e => e.Id == locationId);
        
        // Edge case: if location cannot be found then exit early.
        // This in reality should never happen because ever "company" has at least one default location
        if (locationDTO is null)
            return BadRequest("Default location appears to be missing, check and try again");
        
        // Gather up all player people (teacher) emails
        var emails = records.SelectMany(e => e.Record.UserEmails)
            .ToList();

        // Get all people using the collection of person/player associated emails
        var people = await _mediator.Send(new GetPeopleByEmails
        {
            CompanyId = companyId,
            Emails = emails
        });

        var problemRecords = new List<ProblemRecord<BulkStudentFile>>();
        
        foreach (var player in records)
        {
            var studentDTO = _mapper.Map<StudentDTO>(player.Record);
            studentDTO.LocationId = locationDTO.Id;
            studentDTO.GradeLevel = player.Record.Grade;
            
            // Get person IDs using the player associated emails
            var personIds = new List<Guid>();
            personIds.AddRange(player.Record.UserEmails
                .Where(email => people.ContainsKey(email))
                .Select(email => people[email].Id));

            try
            {
                await _mediator.Send(new AddStudent
                {
                    CompanyId = companyId,
                    StudentDTO = studentDTO,
                    PersonIds = personIds,
                    CustomFieldValueDTO = new CustomFieldValueDTO
                    {
                        CustomFieldValueOne = player.Record.CustomFieldValueOne,
                        CustomFieldValueTwo = player.Record.CustomFieldValueTwo
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception: {Message}", ex.Message);
                problemRecords.Add(new ProblemRecord<BulkStudentFile>
                {
                    Record = player.Record,
                    Message = ex.Message
                });
            }
        }

        return Ok(new
        {
            CsvFile = records,
            FailedToProccessRecord = problemRecords
        });
    }

    public class UploadClientFile
    {
        public Guid LocationId { get; set; }
        public IFormFile? File { get; set; }
    }

    public class ProblemRecord<T>
    {
        public T? Record { get; init; }
        public string Message { get; init; } = null!;
    }
}