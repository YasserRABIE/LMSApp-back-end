using LMS.Application.Common;
using LMS.Application.Features.Content.DTOs;
using MediatR;

namespace LMS.Application.Features.Content.Queries.Tags.GetTagById;

public sealed record GetTagByIdQuery(Guid Id) : IRequest<ApiResult<TagDto>>;
