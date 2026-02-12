using AutoMapper;
using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Application.Features.Content.DTOs;
using MediatR;

namespace LMS.Application.Features.Content.Queries.Tags.GetTagsList;

public sealed class GetTagsListQueryHandler : IRequestHandler<GetTagsListQuery, ApiResult<List<TagDto>>>
{
    private readonly ITagRepository _tagRepository;
    private readonly IMapper _mapper;

    public GetTagsListQueryHandler(
        ITagRepository tagRepository,
        IMapper mapper)
    {
        _tagRepository = tagRepository;
        _mapper = mapper;
    }

    public async Task<ApiResult<List<TagDto>>> Handle(GetTagsListQuery request, CancellationToken cancellationToken)
    {
        var tags = await _tagRepository.GetAllAsync(cancellationToken);
        var tagDtos = _mapper.Map<List<TagDto>>(tags);
        return ApiResult<List<TagDto>>.Ok(tagDtos);
    }
}
