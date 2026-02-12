using AutoMapper;
using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Application.Features.Content.DTOs;
using LMS.Domain.Content;
using MediatR;

namespace LMS.Application.Features.Content.Queries.ContentItems.GetContentItemsList;

public sealed class GetContentItemsListQueryHandler : IRequestHandler<GetContentItemsListQuery, ApiResult<List<ContentItemListDto>>>
{
    private readonly IContentItemRepository _contentItemRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetContentItemsListQueryHandler(
        IContentItemRepository contentItemRepository,
        IProductRepository productRepository,
        IMapper mapper)
    {
        _contentItemRepository = contentItemRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<ApiResult<List<ContentItemListDto>>> Handle(GetContentItemsListQuery request, CancellationToken cancellationToken)
    {
        // Get content items by stage
        var contentItems = await _contentItemRepository.GetContentItemsByStageAsync(
            StageId.From(request.StageId),
            cancellationToken);

        // Map to DTOs
        var contentItemDtos = _mapper.Map<List<ContentItemListDto>>(contentItems);
        var contentItemsList = contentItems.ToList();

        // Enrich with price information
        var enrichedDtos = new List<ContentItemListDto>();
        for (var i = 0; i < contentItemDtos.Count; i++)
        {
            var contentItemDto = contentItemDtos[i];
            var contentItem = contentItemsList[i];

            decimal? price = null;
            if (contentItem.ProductId != null)
            {
                var product = await _productRepository.GetByIdAsync(contentItem.ProductId, cancellationToken);
                price = product?.CashPrice;
            }

            enrichedDtos.Add(contentItemDto with { Price = price });
        }

        return ApiResult<List<ContentItemListDto>>.Ok(enrichedDtos);
    }
}
