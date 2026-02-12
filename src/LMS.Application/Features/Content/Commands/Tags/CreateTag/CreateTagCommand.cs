using LMS.Application.Common;
using MediatR;

namespace LMS.Application.Features.Content.Commands.Tags.CreateTag;

public sealed record CreateTagCommand(string Name) : IRequest<ApiResult<Guid>>;
