using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Domain.Common;
using LMS.Domain.Content;
using MediatR;

namespace LMS.Application.Features.Content.Commands.Stages.UpdateStage;

public sealed class UpdateStageCommandHandler : IRequestHandler<UpdateStageCommand, ApiResult>
{
    private readonly IStageRepository _stageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateStageCommandHandler(
        IStageRepository stageRepository,
        IUnitOfWork unitOfWork)
    {
        _stageRepository = stageRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult> Handle(UpdateStageCommand request, CancellationToken cancellationToken)
    {
        // Get the stage
        var stage = await _stageRepository.GetByIdAsync(StageId.From(request.StageId), cancellationToken);
        if (stage == null)
        {
            return ApiResult.Fail(
                ErrorCodes.Stage.NotFound,
                ErrorMessages.GetMessage(ErrorCodes.Stage.NotFound),
                HttpStatusCodes.NotFound);
        }

        // Update stage details
        var updateResult = stage.UpdateDetails(
            title: request.Title,
            description: request.Description);

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

        return ApiResult.Ok(SuccessMessages.StageUpdated);
    }
}
