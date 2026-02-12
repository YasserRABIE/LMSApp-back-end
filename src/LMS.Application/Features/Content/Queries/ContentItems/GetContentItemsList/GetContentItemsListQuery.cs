using LMS.Application.Common;
using LMS.Application.Features.Content.DTOs;
using MediatR;

namespace LMS.Application.Features.Content.Queries.ContentItems.GetContentItemsList;

public sealed record GetContentItemsListQuery(
    Guid StageId
) : IRequest<ApiResult<List<ContentItemListDto>>>;
