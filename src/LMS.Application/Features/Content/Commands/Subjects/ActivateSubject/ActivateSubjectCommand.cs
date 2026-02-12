using LMS.Application.Common;
using MediatR;

namespace LMS.Application.Features.Content.Commands.Subjects.ActivateSubject;

public sealed record ActivateSubjectCommand(Guid Id) : IRequest<ApiResult>;
