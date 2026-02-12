using AutoMapper;
using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Application.Features.Content.DTOs;
using LMS.Domain.Common;
using LMS.Domain.Content;
using MediatR;

namespace LMS.Application.Features.Content.Queries.Tags.GetTagById;

public sealed class GetTagByIdQueryHandler : IRequestHandler<GetTagByIdQuery, ApiResult<TagDto>>
{
    private readonly ITagRepository _tagRepository;
    private readonly IMapper _mapper;

    public GetTagByIdQueryHandler(
        ITagRepository tagRepository,
        IMapper mapper)
    {
        _tagRepository = tagRepository;
        _mapper = mapper;
    }

    public async Task<ApiResult<TagDto>> Handle(GetTagByIdQuery request, CancellationToken cancellationToken)
    {
        var tag = await _tagRepository.GetByIdAsync(
            TagId.From(request.Id),
            cancellationToken);

        if (tag is null)
        {
            return ApiResult<TagDto>.Fail(
                ErrorCodes.Tag.NotFound,
                ErrorMessages.GetMessage(ErrorCodes.Tag.NotFound),
                404);
        }

        var tagDto = _mapper.Map<TagDto>(tag);
        return ApiResult<TagDto>.Ok(tagDto);
    }
}
