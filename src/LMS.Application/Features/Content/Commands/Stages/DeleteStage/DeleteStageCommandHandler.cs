using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Domain.Common;
using LMS.Domain.Content;
using MediatR;

namespace LMS.Application.Features.Content.Commands.Stages.DeleteStage;

public sealed class DeleteStageCommandHandler : IRequestHandler<DeleteStageCommand, ApiResult>
{
    private readonly IStageRepository _stageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteStageCommandHandler(
        IStageRepository stageRepository,
        IUnitOfWork unitOfWork)
    {
        _stageRepository = stageRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult> Handle(DeleteStageCommand request, CancellationToken cancellationToken)
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

        // Soft delete by deactivating
        stage.Deactivate();

        // Persist changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResult.Ok(SuccessMessages.StageDeleted);
    }
}
