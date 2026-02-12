using AutoMapper;
using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Application.Features.Content.DTOs;
using LMS.Domain.Content;
using MediatR;

namespace LMS.Application.Features.Content.Queries.Stages.GetStagesList;

public sealed class GetStagesListQueryHandler : IRequestHandler<GetStagesListQuery, ApiResult<List<StageListDto>>>
{
    private readonly IStageRepository _stageRepository;
    private readonly IContentItemRepository _contentItemRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetStagesListQueryHandler(
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

    public async Task<ApiResult<List<StageListDto>>> Handle(GetStagesListQuery request, CancellationToken cancellationToken)
    {
        // Get stages by module
        var stages = await _stageRepository.GetStagesByModuleAsync(
            ModuleId.From(request.ModuleId),
            cancellationToken);

        // Map to DTOs
        var stageDtos = _mapper.Map<List<StageListDto>>(stages);
        var stagesList = stages.ToList();

        // Enrich with additional data
        var enrichedDtos = new List<StageListDto>();
        for (var i = 0; i < stageDtos.Count; i++)
        {
            var stageDto = stageDtos[i];
            var stage = stagesList[i];

            // Get content items count
            var contentItems = await _contentItemRepository.GetContentItemsByStageAsync(stage.Id, cancellationToken);

            // Get price
            decimal? price = null;
            if (stage.ProductId != null)
            {
                var product = await _productRepository.GetByIdAsync(stage.ProductId, cancellationToken);
                price = product?.CashPrice;
            }

            enrichedDtos.Add(stageDto with
            {
                ContentItemsCount = contentItems.Count,
                Price = price
            });
        }

        return ApiResult<List<StageListDto>>.Ok(enrichedDtos);
    }
}
