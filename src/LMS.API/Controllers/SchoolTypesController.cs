using LMS.Application.Common;
using LMS.Application.Features.Content.DTOs;
using LMS.Application.Features.Content.Queries.SchoolTypes.GetSchoolTypesList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SchoolTypesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SchoolTypesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResult<List<SchoolTypeDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSchoolTypesList()
    {
        var query = new GetSchoolTypesListQuery();
        var result = await _mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }
}
