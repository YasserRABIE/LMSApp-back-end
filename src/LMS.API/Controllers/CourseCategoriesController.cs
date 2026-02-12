using LMS.Application.Common;
using LMS.Application.Features.Content.DTOs;
using LMS.Application.Features.Content.Queries.CourseCategories.GetCourseCategoriesList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CourseCategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CourseCategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResult<List<CourseCategoryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCourseCategoriesList()
    {
        var query = new GetCourseCategoriesListQuery();
        var result = await _mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }
}
