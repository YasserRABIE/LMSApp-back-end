using LMS.API.Authorization;
using LMS.Application.Common;
using LMS.Application.Features.Content.Commands.Courses.CreateCourse;
using LMS.Application.Features.Content.Commands.Courses.DeleteCourse;
using LMS.Application.Features.Content.Commands.Courses.PublishCourse;
using LMS.Application.Features.Content.Commands.Courses.UpdateCourse;
using LMS.Application.Features.Content.DTOs;
using LMS.Application.Features.Content.Queries.Courses.GetCourseById;
using LMS.Application.Features.Content.Queries.Courses.GetCoursesList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CoursesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = Policies.TeachersAndAdmins)]
    [ProducesResponseType(typeof(ApiResult<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCourse([FromBody] CreateCourseCommand command)
    {
        var result = await _mediator.Send(command);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = Policies.TeachersAndAdmins)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] UpdateCourseRequest request)
    {
        var command = new UpdateCourseCommand(
            id,
            request.SubjectId,
            request.SchoolTypeId,
            request.CourseCategoryId,
            request.Title,
            request.Description,
            request.Thumbnail,
            request.Visibility);

        var result = await _mediator.Send(command);
        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("{id}/publish")]
    [Authorize(Policy = Policies.TeachersAndAdmins)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PublishCourse(Guid id)
    {
        var command = new PublishCourseCommand(id);
        var result = await _mediator.Send(command);
        return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = Policies.TeachersAndAdmins)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCourse(Guid id)
    {
        var command = new DeleteCourseCommand(id);
        var result = await _mediator.Send(command);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResult<CourseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCourseById(Guid id)
    {
        var query = new GetCourseByIdQuery(id);
        var result = await _mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResult<List<CourseListDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCoursesList([FromQuery] Guid subjectId, [FromQuery] Guid? teacherId = null, [FromQuery] bool includeHidden = false)
    {
        var query = new GetCoursesListQuery(subjectId, teacherId, includeHidden);
        var result = await _mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }
}

public record UpdateCourseRequest(
    Guid SubjectId,
    Guid SchoolTypeId,
    Guid CourseCategoryId,
    string Title,
    string Description,
    string Thumbnail,
    string Visibility);
