using LMS.Application.Common;
using LMS.Application.Features.Content.DTOs;
using MediatR;

namespace LMS.Application.Features.Content.Queries.Stages.GetStageById;

public sealed record GetStageByIdQuery(
    Guid StageId
) : IRequest<ApiResult<StageDto>>;
