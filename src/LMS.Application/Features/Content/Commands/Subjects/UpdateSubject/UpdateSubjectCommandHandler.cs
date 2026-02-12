using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Domain.Common;
using LMS.Domain.Content;
using MediatR;

namespace LMS.Application.Features.Content.Commands.Subjects.UpdateSubject;

public sealed class UpdateSubjectCommandHandler : IRequestHandler<UpdateSubjectCommand, ApiResult>
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSubjectCommandHandler(
        ISubjectRepository subjectRepository,
        IUnitOfWork unitOfWork)
    {
        _subjectRepository = subjectRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult> Handle(UpdateSubjectCommand request, CancellationToken cancellationToken)
    {
        var subject = await _subjectRepository.GetByIdAsync(
            SubjectId.From(request.Id),
            cancellationToken);

        if (subject is null)
        {
            return ApiResult.Fail(
                ErrorCodes.Subject.NotFound,
                ErrorMessages.GetMessage(ErrorCodes.Subject.NotFound),
                404);
        }

        var updateResult = subject.UpdateDetails(
            request.Name,
            request.Icon,
            request.Color,
            request.IsCore,
            request.DisplayOrder);

        if (updateResult.IsFailure)
        {
            return ApiResult.Fail(
                updateResult.Error.Code,
                ErrorMessages.GetMessage(updateResult.Error.Code),
                400);
        }

        _subjectRepository.Update(subject);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResult.Ok();
    }
}
