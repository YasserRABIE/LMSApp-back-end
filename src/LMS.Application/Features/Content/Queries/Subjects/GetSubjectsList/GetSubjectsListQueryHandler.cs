using AutoMapper;
using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Application.Features.Content.DTOs;
using MediatR;

namespace LMS.Application.Features.Content.Queries.Subjects.GetSubjectsList;

public sealed class GetSubjectsListQueryHandler : IRequestHandler<GetSubjectsListQuery, ApiResult<List<SubjectListDto>>>
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly IMapper _mapper;

    public GetSubjectsListQueryHandler(
        ISubjectRepository subjectRepository,
        IMapper mapper)
    {
        _subjectRepository = subjectRepository;
        _mapper = mapper;
    }

    public async Task<ApiResult<List<SubjectListDto>>> Handle(GetSubjectsListQuery request, CancellationToken cancellationToken)
    {
        var subjects = request.CoreOnly
            ? await _subjectRepository.GetCoreSubjectsAsync(cancellationToken)
            : request.ActiveOnly
                ? await _subjectRepository.GetActiveSubjectsAsync(cancellationToken)
                : (await _subjectRepository.GetAllAsync(cancellationToken)).OrderBy(s => s.DisplayOrder).ToList();

        var subjectDtos = _mapper.Map<List<SubjectListDto>>(subjects);
        return ApiResult<List<SubjectListDto>>.Ok(subjectDtos);
    }
}
