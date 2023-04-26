using System.Threading.Tasks;
using Application.Common.Models;
using AutoMapper;
using GraphQl.Models.SessionController;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using AuthenticateRequest = GraphQl.Models.SessionController.AuthenticateRequest;

namespace GraphQl.Controllers;

[Route("api/session")]
[ApiController]
public class SessionController : Controller
{
    private readonly StudentService _studentService;
    private readonly IMapper _mapper;

    public SessionController(StudentService studentService, IMapper mapper)
    {
        _studentService = studentService;
        _mapper = mapper;
    }

    /// <summary>
    /// Get a token specifically for a session/student
    /// </summary>
    /// <remarks>
    /// Ideally this would have been a GraphQL Mutation but HC authorization handlers doesn't support
    /// <see>
    ///     <cref>AllowAnonymous</cref>
    /// </see> just yet. So until it's supported this route will be used instead.
    /// </remarks>
    /// <param name="requestModel">Contains values required for student/session validation</param>
    /// <response code="200">Student session authorization is successful</response>
    /// <response code="401">Authorization failed</response>
    /// <returns>Authorization result object</returns>
    [AllowAnonymous]
    [HttpPost("authenticate", Name = nameof(Authenticate))]
    [Consumes("application/json")]
    [Produces("application/json")]
    [SwaggerResponse(StatusCodes.Status200OK, "Student session authorization is successful", typeof(AuthorizeResultDTO))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authorization failed")]
    public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest requestModel)
    {
        var authorizeResultDTO = await _studentService.AuthenticateSession(requestModel.SessionId,
            requestModel.StudentId, requestModel.Code, requestModel.Dob);

        var responseModel = _mapper.Map<AuthenticateResponse>(authorizeResultDTO);
        return authorizeResultDTO.Success ? Ok(responseModel) : Unauthorized(responseModel);
    }
}