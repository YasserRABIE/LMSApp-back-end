using LMS.Application.Common;
using LMS.Application.Features.Content.DTOs;
using MediatR;

namespace LMS.Application.Features.Content.Queries.Modules.GetModulesList;

public sealed record GetModulesListQuery(
    Guid CourseId
) : IRequest<ApiResult<List<ModuleListDto>>>;
