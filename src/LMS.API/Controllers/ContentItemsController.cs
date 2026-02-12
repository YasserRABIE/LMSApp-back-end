using LMS.API.Authorization;
using LMS.Application.Common;
using LMS.Application.Features.Content.Commands.ContentItems.CreateContentItem;
using LMS.Application.Features.Content.Commands.ContentItems.DeleteContentItem;
using LMS.Application.Features.Content.Commands.ContentItems.UpdateContentItem;
using LMS.Application.Features.Content.DTOs;
using LMS.Application.Features.Content.Queries.ContentItems.GetContentItemById;
using LMS.Application.Features.Content.Queries.ContentItems.GetContentItemsList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContentItemsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContentItemsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = Policies.TeachersAndAdmins)]
    [ProducesResponseType(typeof(ApiResult<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateContentItem([FromBody] CreateContentItemCommand command)
    {
        var result = await _mediator.Send(command);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = Policies.TeachersAndAdmins)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateContentItem(Guid id, [FromBody] UpdateContentItemRequest request)
    {
        var command = new UpdateContentItemCommand(
            id,
            request.Title,
            request.Description,
            request.XpReward,
            request.PurchasingPointsReward,
            request.IsFreePreview,
            request.IsActive);

        var result = await _mediator.Send(command);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = Policies.TeachersAndAdmins)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteContentItem(Guid id)
    {
        var command = new DeleteContentItemCommand(id);
        var result = await _mediator.Send(command);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResult<ContentItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetContentItemById(Guid id)
    {
        var query = new GetContentItemByIdQuery(id);
        var result = await _mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResult<List<ContentItemListDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetContentItemsList([FromQuery] Guid stageId)
    {
        var query = new GetContentItemsListQuery(stageId);
        var result = await _mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }
}

public record UpdateContentItemRequest(
    string Title,
    string? Description,
    int XpReward,
    int PurchasingPointsReward,
    bool IsFreePreview,
    bool IsActive);
