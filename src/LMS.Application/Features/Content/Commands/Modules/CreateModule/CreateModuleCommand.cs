using LMS.Application.Common;
using MediatR;

namespace LMS.Application.Features.Content.Commands.Modules.CreateModule;

public sealed record CreateModuleCommand(
    Guid CourseId,
    string Title,
    string? Description,
    int DisplayOrder,
    decimal EstimatedHours,
    string? Thumbnail
) : IRequest<ApiResult<Guid>>;
