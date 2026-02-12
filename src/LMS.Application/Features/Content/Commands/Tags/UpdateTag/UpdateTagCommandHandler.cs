using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Domain.Common;
using LMS.Domain.Content;
using MediatR;

namespace LMS.Application.Features.Content.Commands.Tags.UpdateTag;

public sealed class UpdateTagCommandHandler : IRequestHandler<UpdateTagCommand, ApiResult>
{
    private readonly ITagRepository _tagRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTagCommandHandler(
        ITagRepository tagRepository,
        IUnitOfWork unitOfWork)
    {
        _tagRepository = tagRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
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

        var existingTag = await _tagRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existingTag is not null && existingTag.Id != tag.Id)
        {
            return ApiResult.Fail(
                ErrorCodes.Tag.AlreadyExists,
                ErrorMessages.GetMessage(ErrorCodes.Tag.AlreadyExists),
                400);
        }

        var updateResult = tag.UpdateName(request.Name);

        if (updateResult.IsFailure)
        {
            return ApiResult.Fail(
                updateResult.Error.Code,
                ErrorMessages.GetMessage(updateResult.Error.Code),
                400);
        }

        _tagRepository.Update(tag);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResult.Ok();
    }
}
