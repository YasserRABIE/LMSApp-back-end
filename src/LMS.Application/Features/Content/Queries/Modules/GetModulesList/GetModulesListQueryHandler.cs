using AutoMapper;
using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Application.Features.Content.DTOs;
using LMS.Domain.Content;
using MediatR;

namespace LMS.Application.Features.Content.Queries.Modules.GetModulesList;

public sealed class GetModulesListQueryHandler : IRequestHandler<GetModulesListQuery, ApiResult<List<ModuleListDto>>>
{
    private readonly IModuleRepository _moduleRepository;
    private readonly IStageRepository _stageRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetModulesListQueryHandler(
        IModuleRepository moduleRepository,
        IStageRepository stageRepository,
        IProductRepository productRepository,
        IMapper mapper)
    {
        _moduleRepository = moduleRepository;
        _stageRepository = stageRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<ApiResult<List<ModuleListDto>>> Handle(GetModulesListQuery request, CancellationToken cancellationToken)
    {
        // Get modules by course
        var modules = await _moduleRepository.GetModulesByCourseAsync(
            CourseId.From(request.CourseId),
            cancellationToken);

        // Map to DTOs
        var moduleDtos = _mapper.Map<List<ModuleListDto>>(modules);
        var modulesList = modules.ToList();

        // Enrich with additional data
        var enrichedDtos = new List<ModuleListDto>();
        for (var i = 0; i < moduleDtos.Count; i++)
        {
            var moduleDto = moduleDtos[i];
            var module = modulesList[i];

            // Get stages count
            var stages = await _stageRepository.GetStagesByModuleAsync(module.Id, cancellationToken);

            // Get price
            decimal? price = null;
            if (module.ProductId != null)
            {
                var product = await _productRepository.GetByIdAsync(module.ProductId, cancellationToken);
                price = product?.CashPrice;
            }

            enrichedDtos.Add(moduleDto with
            {
                StagesCount = stages.Count,
                Price = price
            });
        }

        return ApiResult<List<ModuleListDto>>.Ok(enrichedDtos);
    }
}
