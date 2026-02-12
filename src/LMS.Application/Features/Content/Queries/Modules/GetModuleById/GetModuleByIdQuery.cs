using LMS.Application.Common;
using LMS.Application.Features.Content.DTOs;
using MediatR;

namespace LMS.Application.Features.Content.Queries.Modules.GetModuleById;

public sealed record GetModuleByIdQuery(
    Guid ModuleId
) : IRequest<ApiResult<ModuleDto>>;
