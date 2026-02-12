using LMS.Application.Common;
using LMS.Application.Features.Content.DTOs;
using MediatR;

namespace LMS.Application.Features.Content.Queries.Stages.GetStagesList;

public sealed record GetStagesListQuery(
    Guid ModuleId
) : IRequest<ApiResult<List<StageListDto>>>;
