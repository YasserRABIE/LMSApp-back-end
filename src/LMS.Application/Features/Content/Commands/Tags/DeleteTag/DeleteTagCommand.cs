using LMS.Application.Common;
using MediatR;

namespace LMS.Application.Features.Content.Commands.Tags.DeleteTag;

public sealed record DeleteTagCommand(Guid Id) : IRequest<ApiResult>;
