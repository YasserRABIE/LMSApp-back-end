using LMS.API.Authorization;
using LMS.Application.Common;
using LMS.Application.Features.Content.Commands.Subjects.ActivateSubject;
using LMS.Application.Features.Content.Commands.Subjects.CreateSubject;
using LMS.Application.Features.Content.Commands.Subjects.DeactivateSubject;
using LMS.Application.Features.Content.Commands.Subjects.UpdateSubject;
using LMS.Application.Features.Content.DTOs;
using LMS.Application.Features.Content.Queries.Subjects.GetSubjectById;
using LMS.Application.Features.Content.Queries.Subjects.GetSubjectsList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubjectsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SubjectsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = Policies.AdminOnly)]
    [ProducesResponseType(typeof(ApiResult<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectCommand command)
    {
        var result = await _mediator.Send(command);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = Policies.AdminOnly)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSubject(Guid id, [FromBody] UpdateSubjectRequest request)
    {
        var command = new UpdateSubjectCommand(
            id,
            request.Name,
            request.Icon,
            request.Color,
            request.IsCore,
            request.DisplayOrder);

        var result = await _mediator.Send(command);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("{id}/activate")]
    [Authorize(Policy = Policies.AdminOnly)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateSubject(Guid id)
    {
        var command = new ActivateSubjectCommand(id);
        var result = await _mediator.Send(command);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("{id}/deactivate")]
    [Authorize(Policy = Policies.AdminOnly)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateSubject(Guid id)
    {
        var command = new DeactivateSubjectCommand(id);
        var result = await _mediator.Send(command);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResult<SubjectDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSubjectById(Guid id)
    {
        var query = new GetSubjectByIdQuery(id);
        var result = await _mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResult<List<SubjectListDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSubjectsList([FromQuery] bool activeOnly = true, [FromQuery] bool coreOnly = false)
    {
        var query = new GetSubjectsListQuery(activeOnly, coreOnly);
        var result = await _mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }
}

public record UpdateSubjectRequest(
    string Name,
    string Icon,
    string Color,
    bool IsCore,
    int DisplayOrder);
