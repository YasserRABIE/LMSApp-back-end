using LMS.Application.Common;
using MediatR;

namespace LMS.Application.Features.Content.Commands.Stages.UpdateStage;

public sealed record UpdateStageCommand(
    Guid StageId,
    string Title,
    string? Description
) : IRequest<ApiResult>;
