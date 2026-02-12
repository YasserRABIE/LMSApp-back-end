using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Domain.Common;
using LMS.Domain.Content;
using MediatR;

namespace LMS.Application.Features.Content.Commands.Subjects.ActivateSubject;

public sealed class ActivateSubjectCommandHandler : IRequestHandler<ActivateSubjectCommand, ApiResult>
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateSubjectCommandHandler(
        ISubjectRepository subjectRepository,
        IUnitOfWork unitOfWork)
    {
        _subjectRepository = subjectRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult> Handle(ActivateSubjectCommand request, CancellationToken cancellationToken)
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

        subject.Activate();

        _subjectRepository.Update(subject);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResult.Ok();
    }
}
