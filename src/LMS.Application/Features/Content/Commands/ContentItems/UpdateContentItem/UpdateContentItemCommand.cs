using LMS.Application.Common;
using MediatR;

namespace LMS.Application.Features.Content.Commands.ContentItems.UpdateContentItem;

public sealed record UpdateContentItemCommand(
    Guid ContentItemId,
    string Title,
    string? Description,
    int XpReward,
    int PurchasingPointsReward,
    bool IsFreePreview,
    bool IsActive
) : IRequest<ApiResult<Unit>>;
