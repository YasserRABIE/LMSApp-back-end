using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Domain.Common;
using LMS.Domain.Content;
using MediatR;

namespace LMS.Application.Features.Content.Commands.ContentItems.CreateContentItem;

public sealed class CreateContentItemCommandHandler : IRequestHandler<CreateContentItemCommand, ApiResult<Guid>>
{
    private readonly IContentItemRepository _contentItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateContentItemCommandHandler(
        IContentItemRepository contentItemRepository,
        IUnitOfWork unitOfWork)
    {
        _contentItemRepository = contentItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult<Guid>> Handle(CreateContentItemCommand request, CancellationToken cancellationToken)
    {
        // Parse ContentType enum
        if (!Enum.TryParse<ContentType>(request.ContentType, true, out var contentType))
        {
            return ApiResult<Guid>.Fail(
                ErrorCodes.Content.InvalidContentType,
                ErrorMessages.GetMessage(ErrorCodes.Content.InvalidContentType),
                HttpStatusCodes.BadRequest);
        }

        // Create content item
        var contentItemResult = ContentItem.Create(
            StageId.From(request.StageId),
            contentType,
            request.Title,
            request.Description,
            request.DisplayOrder,
            request.XpReward,
            request.PurchasingPointsReward);

        if (contentItemResult.IsFailure)
        {
            var statusCode = contentItemResult.Error.Type switch
            {
                ErrorType.Validation => HttpStatusCodes.BadRequest,
                ErrorType.NotFound => HttpStatusCodes.NotFound,
                ErrorType.Conflict => HttpStatusCodes.Conflict,
                ErrorType.Unauthorized => HttpStatusCodes.Unauthorized,
                ErrorType.Forbidden => HttpStatusCodes.Forbidden,
                _ => HttpStatusCodes.BadRequest
            };

            return ApiResult<Guid>.Fail(
                contentItemResult.Error.Code,
                ErrorMessages.GetMessage(contentItemResult.Error.Code),
                statusCode);
        }

        await _contentItemRepository.AddAsync(contentItemResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResult<Guid>.Ok(
            contentItemResult.Value.Id.Value,
            SuccessMessages.ContentItemCreated,
            HttpStatusCodes.Created);
    }
}
