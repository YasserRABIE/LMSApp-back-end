using AutoMapper;
using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Application.Features.Content.DTOs;
using LMS.Domain.Common;
using LMS.Domain.Content;
using MediatR;

namespace LMS.Application.Features.Content.Queries.Subjects.GetSubjectById;

public sealed class GetSubjectByIdQueryHandler : IRequestHandler<GetSubjectByIdQuery, ApiResult<SubjectDto>>
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly IMapper _mapper;

    public GetSubjectByIdQueryHandler(
        ISubjectRepository subjectRepository,
        IMapper mapper)
    {
        _subjectRepository = subjectRepository;
        _mapper = mapper;
    }

    public async Task<ApiResult<SubjectDto>> Handle(GetSubjectByIdQuery request, CancellationToken cancellationToken)
    {
        var subject = await _subjectRepository.GetByIdAsync(
            SubjectId.From(request.Id),
            cancellationToken);

        if (subject is null)
        {
            return ApiResult<SubjectDto>.Fail(
                ErrorCodes.Subject.NotFound,
                ErrorMessages.GetMessage(ErrorCodes.Subject.NotFound),
                404);
        }

        var subjectDto = _mapper.Map<SubjectDto>(subject);
        return ApiResult<SubjectDto>.Ok(subjectDto);
    }
}
