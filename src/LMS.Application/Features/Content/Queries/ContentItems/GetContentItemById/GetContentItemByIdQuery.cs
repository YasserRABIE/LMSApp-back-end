using LMS.Application.Common;
using LMS.Application.Features.Content.DTOs;
using MediatR;

namespace LMS.Application.Features.Content.Queries.ContentItems.GetContentItemById;

public sealed record GetContentItemByIdQuery(
    Guid ContentItemId
) : IRequest<ApiResult<ContentItemDto>>;
