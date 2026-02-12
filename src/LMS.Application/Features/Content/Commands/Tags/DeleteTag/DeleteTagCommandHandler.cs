using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Domain.Common;
using LMS.Domain.Content;
using MediatR;

namespace LMS.Application.Features.Content.Commands.Tags.DeleteTag;

public sealed class DeleteTagCommandHandler : IRequestHandler<DeleteTagCommand, ApiResult>
{
    private readonly ITagRepository _tagRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTagCommandHandler(
        ITagRepository tagRepository,
        IUnitOfWork unitOfWork)
    {
        _tagRepository = tagRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await _tagRepository.GetByIdAsync(
            TagId.From(request.Id),
            cancellationToken);

        if (tag is null)
        {
            return ApiResult.Fail(
                ErrorCodes.Tag.NotFound,
                ErrorMessages.GetMessage(ErrorCodes.Tag.NotFound),
                404);
        }

        _tagRepository.Remove(tag);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResult.Ok();
    }
}
