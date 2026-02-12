using LMS.Application.Common;
using MediatR;

namespace LMS.Application.Features.Content.Commands.ContentItems.DeleteContentItem;

public sealed record DeleteContentItemCommand(
    Guid ContentItemId
) : IRequest<ApiResult<Unit>>;
