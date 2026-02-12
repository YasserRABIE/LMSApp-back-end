using LMS.Application.Common;
using MediatR;

namespace LMS.Application.Features.Content.Commands.Modules.DeleteModule;

public sealed record DeleteModuleCommand(
    Guid ModuleId
) : IRequest<ApiResult>;
