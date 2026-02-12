using LMS.Application.Common;
using LMS.Application.Features.Content.DTOs;
using MediatR;

namespace LMS.Application.Features.Content.Queries.Subjects.GetSubjectsList;

public sealed record GetSubjectsListQuery(
    bool ActiveOnly = true,
    bool CoreOnly = false
) : IRequest<ApiResult<List<SubjectListDto>>>;
