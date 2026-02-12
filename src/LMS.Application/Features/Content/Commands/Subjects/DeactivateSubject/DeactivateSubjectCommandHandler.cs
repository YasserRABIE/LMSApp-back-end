using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Domain.Common;
using LMS.Domain.Content;
using MediatR;

namespace LMS.Application.Features.Content.Commands.Subjects.DeactivateSubject;

public sealed class DeactivateSubjectCommandHandler : IRequestHandler<DeactivateSubjectCommand, ApiResult>
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateSubjectCommandHandler(
        ISubjectRepository subjectRepository,
        IUnitOfWork unitOfWork)
    {
        _subjectRepository = subjectRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult> Handle(DeactivateSubjectCommand request, CancellationToken cancellationToken)
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

        subject.Deactivate();

        _subjectRepository.Update(subject);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResult.Ok();
    }
}
