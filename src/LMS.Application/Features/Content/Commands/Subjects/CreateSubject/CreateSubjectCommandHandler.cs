using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Domain.Content;
using MediatR;

namespace LMS.Application.Features.Content.Commands.Subjects.CreateSubject;

public sealed class CreateSubjectCommandHandler : IRequestHandler<CreateSubjectCommand, ApiResult<Guid>>
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSubjectCommandHandler(
        ISubjectRepository subjectRepository,
        IUnitOfWork unitOfWork)
    {
        _subjectRepository = subjectRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult<Guid>> Handle(CreateSubjectCommand request, CancellationToken cancellationToken)
    {
        var subjectResult = Subject.Create(
            request.Name,
            request.Icon,
            request.Color,
            request.IsCore,
            request.DisplayOrder);

        if (subjectResult.IsFailure)
        {
            return ApiResult<Guid>.Fail(
                subjectResult.Error.Code,
                ErrorMessages.GetMessage(subjectResult.Error.Code),
                400);
        }

        await _subjectRepository.AddAsync(subjectResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResult<Guid>.Ok(subjectResult.Value.Id.Value, statusCode: 201);
    }
}
