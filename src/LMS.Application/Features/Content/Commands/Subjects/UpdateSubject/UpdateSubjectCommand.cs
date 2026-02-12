using LMS.Application.Common;
using MediatR;

namespace LMS.Application.Features.Content.Commands.Subjects.UpdateSubject;

public sealed record UpdateSubjectCommand(
    Guid Id,
    string Name,
    string Icon,
    string Color,
    bool IsCore,
    int DisplayOrder
) : IRequest<ApiResult>;
