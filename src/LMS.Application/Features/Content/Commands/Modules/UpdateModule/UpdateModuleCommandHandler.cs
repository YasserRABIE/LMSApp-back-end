using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Domain.Common;
using LMS.Domain.Content;
using MediatR;

namespace LMS.Application.Features.Content.Commands.Modules.UpdateModule;

public sealed class UpdateModuleCommandHandler : IRequestHandler<UpdateModuleCommand, ApiResult>
{
    private readonly IModuleRepository _moduleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateModuleCommandHandler(
        IModuleRepository moduleRepository,
        IUnitOfWork unitOfWork)
    {
        _moduleRepository = moduleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult> Handle(UpdateModuleCommand request, CancellationToken cancellationToken)
    {
        // Get the module
        var module = await _moduleRepository.GetByIdAsync(ModuleId.From(request.ModuleId), cancellationToken);
        if (module == null)
        {
            return ApiResult.Fail(
                ErrorCodes.Module.NotFound,
                ErrorMessages.GetMessage(ErrorCodes.Module.NotFound),
                HttpStatusCodes.NotFound);
        }

        // Update module details
        var updateResult = module.UpdateDetails(
            title: request.Title,
            description: request.Description,
            estimatedHours: request.EstimatedHours,
            thumbnail: request.Thumbnail);

        if (updateResult.IsFailure)
        {
            var statusCode = updateResult.Error.Type switch
            {
                ErrorType.Validation => HttpStatusCodes.BadRequest,
                ErrorType.NotFound => HttpStatusCodes.NotFound,
                ErrorType.Conflict => HttpStatusCodes.Conflict,
                ErrorType.Unauthorized => HttpStatusCodes.Unauthorized,
                ErrorType.Forbidden => HttpStatusCodes.Forbidden,
                _ => HttpStatusCodes.BadRequest
            };

            return ApiResult.Fail(
                updateResult.Error.Code,
                ErrorMessages.GetMessage(updateResult.Error.Code),
                statusCode);
        }

        // Persist changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResult.Ok(SuccessMessages.ModuleUpdated);
    }
}
