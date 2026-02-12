using AutoMapper;
using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Application.Features.Content.DTOs;
using LMS.Domain.Common;
using LMS.Domain.Content;
using MediatR;

namespace LMS.Application.Features.Content.Queries.Stages.GetStageById;

public sealed class GetStageByIdQueryHandler : IRequestHandler<GetStageByIdQuery, ApiResult<StageDto>>
{
    private readonly IStageRepository _stageRepository;
    private readonly IContentItemRepository _contentItemRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetStageByIdQueryHandler(
        IStageRepository stageRepository,
        IContentItemRepository contentItemRepository,
        IProductRepository productRepository,
        IMapper mapper)
    {
        _stageRepository = stageRepository;
        _contentItemRepository = contentItemRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<ApiResult<StageDto>> Handle(GetStageByIdQuery request, CancellationToken cancellationToken)
    {
        // Get the stage
        var stage = await _stageRepository.GetByIdAsync(StageId.From(request.StageId), cancellationToken);
        if (stage == null)
        {
            return ApiResult<StageDto>.Fail(
                ErrorCodes.Stage.NotFound,
                ErrorMessages.GetMessage(ErrorCodes.Stage.NotFound),
                HttpStatusCodes.NotFound);
        }

        // Map to DTO
        var stageDto = _mapper.Map<StageDto>(stage);

        // Get content items
        var contentItems = await _contentItemRepository.GetContentItemsByStageAsync(stage.Id, cancellationToken);
        stageDto = stageDto with { ContentItemsCount = contentItems.Count };

        // Get content items list
        var contentItemDtos = _mapper.Map<List<ContentItemListDto>>(contentItems);
        stageDto = stageDto with { ContentItems = contentItemDtos };

        // Get price if product exists
        if (stage.ProductId != null)
        {
            var product = await _productRepository.GetByIdAsync(stage.ProductId, cancellationToken);
            stageDto = stageDto with { Price = product?.CashPrice };
        }

        return ApiResult<StageDto>.Ok(stageDto);
    }
}
