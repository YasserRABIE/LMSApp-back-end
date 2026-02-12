using LMS.API.Authorization;
using LMS.Application.Common;
using LMS.Application.Features.Content.Commands.Stages.CreateStage;
using LMS.Application.Features.Content.Commands.Stages.DeleteStage;
using LMS.Application.Features.Content.Commands.Stages.UpdateStage;
using LMS.Application.Features.Content.DTOs;
using LMS.Application.Features.Content.Queries.Stages.GetStageById;
using LMS.Application.Features.Content.Queries.Stages.GetStagesList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StagesController : ControllerBase
{
    private readonly IMediator _mediator;

    public StagesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = Policies.TeachersAndAdmins)]
    [ProducesResponseType(typeof(ApiResult<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateStage([FromBody] CreateStageCommand command)
    {
        var result = await _mediator.Send(command);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = Policies.TeachersAndAdmins)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStage(Guid id, [FromBody] UpdateStageRequest request)
    {
        var command = new UpdateStageCommand(
            id,
            request.Title,
            request.Description);

        var result = await _mediator.Send(command);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = Policies.TeachersAndAdmins)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteStage(Guid id)
    {
        var command = new DeleteStageCommand(id);
        var result = await _mediator.Send(command);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResult<StageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStageById(Guid id)
    {
        var query = new GetStageByIdQuery(id);
        var result = await _mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResult<List<StageListDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStagesList([FromQuery] Guid moduleId)
    {
        var query = new GetStagesListQuery(moduleId);
        var result = await _mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }
}

public record UpdateStageRequest(
    string Title,
    string? Description);
