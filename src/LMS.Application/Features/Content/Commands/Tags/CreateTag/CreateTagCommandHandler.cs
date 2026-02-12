using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Domain.Common;
using LMS.Domain.Content;
using MediatR;

namespace LMS.Application.Features.Content.Commands.Tags.CreateTag;

public sealed class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, ApiResult<Guid>>
{
    private readonly ITagRepository _tagRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTagCommandHandler(
        ITagRepository tagRepository,
        IUnitOfWork unitOfWork)
    {
        _tagRepository = tagRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult<Guid>> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var existingTag = await _tagRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existingTag is not null)
        {
            return ApiResult<Guid>.Fail(
                ErrorCodes.Tag.AlreadyExists,
                ErrorMessages.GetMessage(ErrorCodes.Tag.AlreadyExists),
                400);
        }

        var tagResult = Tag.Create(request.Name);

        if (tagResult.IsFailure)
        {
            return ApiResult<Guid>.Fail(
                tagResult.Error.Code,
                ErrorMessages.GetMessage(tagResult.Error.Code),
                400);
        }

        await _tagRepository.AddAsync(tagResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResult<Guid>.Ok(tagResult.Value.Id.Value, statusCode: 201);
    }
}
