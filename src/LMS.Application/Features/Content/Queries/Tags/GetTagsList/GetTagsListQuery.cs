using LMS.Application.Common;
using LMS.Application.Features.Content.DTOs;
using MediatR;

namespace LMS.Application.Features.Content.Queries.Tags.GetTagsList;

public sealed record GetTagsListQuery : IRequest<ApiResult<List<TagDto>>>;
