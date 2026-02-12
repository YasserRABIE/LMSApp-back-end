using LMS.Application.Common;
using LMS.Application.Features.Content.DTOs;
using MediatR;

namespace LMS.Application.Features.Content.Queries.Subjects.GetSubjectById;

public sealed record GetSubjectByIdQuery(Guid Id) : IRequest<ApiResult<SubjectDto>>;
