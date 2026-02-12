using LMS.Application.Common;
using MediatR;

namespace LMS.Application.Features.Content.Commands.Subjects.DeactivateSubject;

public sealed record DeactivateSubjectCommand(Guid Id) : IRequest<ApiResult>;
