using AutoMapper;
using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Application.Features.Content.DTOs;
using LMS.Domain.Common;
using LMS.Domain.Content;
using MediatR;

namespace LMS.Application.Features.Content.Queries.ContentItems.GetContentItemById;

public sealed class GetContentItemByIdQueryHandler : IRequestHandler<GetContentItemByIdQuery, ApiResult<ContentItemDto>>
{
    private readonly IContentItemRepository _contentItemRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetContentItemByIdQueryHandler(
        IContentItemRepository contentItemRepository,
        IProductRepository productRepository,
        IMapper mapper)
    {
        _contentItemRepository = contentItemRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<ApiResult<ContentItemDto>> Handle(GetContentItemByIdQuery request, CancellationToken cancellationToken)
    {
        // Get the content item
        var contentItem = await _contentItemRepository.GetByIdAsync(
            ContentItemId.From(request.ContentItemId),
            cancellationToken);

        if (contentItem == null)
        {
            return ApiResult<ContentItemDto>.Fail(
                ErrorCodes.Content.NotFound,
                ErrorMessages.GetMessage(ErrorCodes.Content.NotFound),
                HttpStatusCodes.NotFound);
        }

        // Map to DTO
        var contentItemDto = _mapper.Map<ContentItemDto>(contentItem);

        // Get price if product exists
        if (contentItem.ProductId != null)
        {
            var product = await _productRepository.GetByIdAsync(contentItem.ProductId, cancellationToken);
            contentItemDto = contentItemDto with { Price = product?.CashPrice };
        }

        return ApiResult<ContentItemDto>.Ok(contentItemDto);
    }
}
