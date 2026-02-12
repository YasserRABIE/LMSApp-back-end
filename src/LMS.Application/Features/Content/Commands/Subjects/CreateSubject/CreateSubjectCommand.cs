using LMS.Application.Common;
using MediatR;

namespace LMS.Application.Features.Content.Commands.Subjects.CreateSubject;

public sealed record CreateSubjectCommand(
    string Name,
    string Icon,
    string Color,
    bool IsCore,
    int DisplayOrder
) : IRequest<ApiResult<Guid>>;
