using LMS.Application.Common;
using MediatR;

namespace LMS.Application.Features.Content.Commands.Stages.CreateStage;

public sealed record CreateStageCommand(
    Guid ModuleId,
    string Title,
    string? Description,
    int DisplayOrder
) : IRequest<ApiResult<Guid>>;
