using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Domain.Common;
using LMS.Domain.Content;
using MediatR;

namespace LMS.Application.Features.Content.Commands.ContentItems.DeleteContentItem;

public sealed class DeleteContentItemCommandHandler : IRequestHandler<DeleteContentItemCommand, ApiResult<Unit>>
{
    private readonly IContentItemRepository _contentItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteContentItemCommandHandler(
        IContentItemRepository contentItemRepository,
        IUnitOfWork unitOfWork)
    {
        _contentItemRepository = contentItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult<Unit>> Handle(DeleteContentItemCommand request, CancellationToken cancellationToken)
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

        _contentItemRepository.Remove(contentItem);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResult<Unit>.Ok(Unit.Value, SuccessMessages.ContentItemDeleted);
    }
}
