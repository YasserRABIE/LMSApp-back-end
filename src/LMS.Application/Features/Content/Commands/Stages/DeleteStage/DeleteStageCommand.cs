using LMS.Application.Common;
using MediatR;

namespace LMS.Application.Features.Content.Commands.Stages.DeleteStage;

public sealed record DeleteStageCommand(
    Guid StageId
) : IRequest<ApiResult>;
