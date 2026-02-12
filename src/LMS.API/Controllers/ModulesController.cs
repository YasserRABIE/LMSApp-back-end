using LMS.API.Authorization;
using LMS.Application.Common;
using LMS.Application.Features.Content.Commands.Modules.CreateModule;
using LMS.Application.Features.Content.Commands.Modules.DeleteModule;
using LMS.Application.Features.Content.Commands.Modules.UpdateModule;
using LMS.Application.Features.Content.DTOs;
using LMS.Application.Features.Content.Queries.Modules.GetModuleById;
using LMS.Application.Features.Content.Queries.Modules.GetModulesList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ModulesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ModulesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = Policies.TeachersAndAdmins)]
    [ProducesResponseType(typeof(ApiResult<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateModule([FromBody] CreateModuleCommand command)
    {
        var result = await _mediator.Send(command);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = Policies.TeachersAndAdmins)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateModule(Guid id, [FromBody] UpdateModuleRequest request)
    {
        var command = new UpdateModuleCommand(
            id,
            request.Title,
            request.Description,
            request.EstimatedHours,
            request.Thumbnail);

        var result = await _mediator.Send(command);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = Policies.TeachersAndAdmins)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteModule(Guid id)
    {
        var command = new DeleteModuleCommand(id);
        var result = await _mediator.Send(command);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResult<ModuleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetModuleById(Guid id)
    {
        var query = new GetModuleByIdQuery(id);
        var result = await _mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResult<List<ModuleListDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetModulesList([FromQuery] Guid courseId)
    {
        var query = new GetModulesListQuery(courseId);
        var result = await _mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }
}

public record UpdateModuleRequest(
    string Title,
    string? Description,
    int EstimatedHours,
    string? Thumbnail);
