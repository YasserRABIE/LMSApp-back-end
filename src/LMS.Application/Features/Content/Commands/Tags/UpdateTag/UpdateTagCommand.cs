using LMS.Application.Common;
using MediatR;

namespace LMS.Application.Features.Content.Commands.Tags.UpdateTag;

public sealed record UpdateTagCommand(Guid Id, string Name) : IRequest<ApiResult>;
