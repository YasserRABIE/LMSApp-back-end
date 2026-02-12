using LMS.Application.Common;
using LMS.Application.Features.Content.DTOs;
using MediatR;

namespace LMS.Application.Features.Content.Queries.SchoolTypes.GetSchoolTypesList;

public sealed record GetSchoolTypesListQuery : IRequest<ApiResult<List<SchoolTypeDto>>>;
