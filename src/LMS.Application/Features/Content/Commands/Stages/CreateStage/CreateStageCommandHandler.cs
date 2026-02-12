using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Domain.Common;
using LMS.Domain.Content;
using MediatR;

namespace LMS.Application.Features.Content.Commands.Stages.CreateStage;

public sealed class CreateStageCommandHandler : IRequestHandler<CreateStageCommand, ApiResult<Guid>>
{
    private readonly IStageRepository _stageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateStageCommandHandler(
        IStageRepository stageRepository,
        IUnitOfWork _unitOfWork)
    {
        _stageRepository = stageRepository;
        this._unitOfWork = _unitOfWork;
    }

    public async Task<ApiResult<Guid>> Handle(CreateStageCommand request, CancellationToken cancellationToken)
    {
        // Create stage using domain factory method
        var stageResult = Stage.Create(
            moduleId: ModuleId.From(request.ModuleId),
            title: request.Title,
            description: request.Description,
            displayOrder: request.DisplayOrder);

        if (stageResult.IsFailure)
        {
            var statusCode = stageResult.Error.Type switch
            {
                ErrorType.Validation => HttpStatusCodes.BadRequest,
                ErrorType.NotFound => HttpStatusCodes.NotFound,
                ErrorType.Conflict => HttpStatusCodes.Conflict,
                ErrorType.Unauthorized => HttpStatusCodes.Unauthorized,
                ErrorType.Forbidden => HttpStatusCodes.Forbidden,
                _ => HttpStatusCodes.BadRequest
            };

            return ApiResult<Guid>.Fail(
                stageResult.Error.Code,
                ErrorMessages.GetMessage(stageResult.Error.Code),
                statusCode);
        }

        // Persist the stage
        await _stageRepository.AddAsync(stageResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResult<Guid>.Ok(
            stageResult.Value.Id.Value,
            SuccessMessages.StageCreated,
            HttpStatusCodes.Created);
    }
}
