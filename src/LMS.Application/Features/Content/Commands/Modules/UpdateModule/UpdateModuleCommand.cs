using LMS.Application.Common;
using MediatR;

namespace LMS.Application.Features.Content.Commands.Modules.UpdateModule;

public sealed record UpdateModuleCommand(
    Guid ModuleId,
    string Title,
    string? Description,
    decimal EstimatedHours,
    string? Thumbnail
) : IRequest<ApiResult>;
