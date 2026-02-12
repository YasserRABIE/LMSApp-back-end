using LMS.Application.Common;
using MediatR;

namespace LMS.Application.Features.Content.Commands.ContentItems.CreateContentItem;

public sealed record CreateContentItemCommand(
    Guid StageId,
    string ContentType,
    string Title,
    string? Description,
    int DisplayOrder,
    int XpReward,
    int PurchasingPointsReward
) : IRequest<ApiResult<Guid>>;
