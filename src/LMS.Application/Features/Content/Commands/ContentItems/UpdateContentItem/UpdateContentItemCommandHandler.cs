using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Domain.Common;
using LMS.Domain.Content;
using MediatR;

namespace LMS.Application.Features.Content.Commands.ContentItems.UpdateContentItem;

public sealed class UpdateContentItemCommandHandler : IRequestHandler<UpdateContentItemCommand, ApiResult<Unit>>
{
    private readonly IContentItemRepository _contentItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateContentItemCommandHandler(
        IContentItemRepository contentItemRepository,
        IUnitOfWork unitOfWork)
    {
        _contentItemRepository = contentItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult<Unit>> Handle(UpdateContentItemCommand request, CancellationToken cancellationToken)
    {
        // Get the content item
        var contentItem = await _contentItemRepository.GetByIdAsync(
            ContentItemId.From(request.ContentItemId),
            cancellationToken);

        if (contentItem == null)
        {
            return ApiResult<Unit>.Fail(
                ErrorCodes.Content.NotFound,
                ErrorMessages.GetMessage(ErrorCodes.Content.NotFound),
                HttpStatusCodes.NotFound);
        }

        // Update details
        var updateResult = contentItem.UpdateDetails(request.Title, request.Description);
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

            return ApiResult<Unit>.Fail(
                updateResult.Error.Code,
                ErrorMessages.GetMessage(updateResult.Error.Code),
                statusCode);
        }

        // Set rewards
        var rewardsResult = contentItem.SetRewards(request.XpReward, request.PurchasingPointsReward);
        if (rewardsResult.IsFailure)
        {
            var statusCode = rewardsResult.Error.Type switch
            {
                ErrorType.Validation => HttpStatusCodes.BadRequest,
                ErrorType.NotFound => HttpStatusCodes.NotFound,
                ErrorType.Conflict => HttpStatusCodes.Conflict,
                ErrorType.Unauthorized => HttpStatusCodes.Unauthorized,
                ErrorType.Forbidden => HttpStatusCodes.Forbidden,
                _ => HttpStatusCodes.BadRequest
            };

            return ApiResult<Unit>.Fail(
                rewardsResult.Error.Code,
                ErrorMessages.GetMessage(rewardsResult.Error.Code),
                statusCode);
        }

        // Update free preview status
        if (request.IsFreePreview)
            contentItem.EnableFreePreview();
        else
            contentItem.DisableFreePreview();

        // Update active status
        if (request.IsActive)
            contentItem.Activate();
        else
            contentItem.Deactivate();

        _contentItemRepository.Update(contentItem);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResult<Unit>.Ok(Unit.Value, SuccessMessages.ContentItemUpdated);
    }
}
