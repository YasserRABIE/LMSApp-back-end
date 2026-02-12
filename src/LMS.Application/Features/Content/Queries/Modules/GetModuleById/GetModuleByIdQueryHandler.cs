using AutoMapper;
using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Application.Features.Content.DTOs;
using LMS.Domain.Common;
using LMS.Domain.Content;
using MediatR;

namespace LMS.Application.Features.Content.Queries.Modules.GetModuleById;

public sealed class GetModuleByIdQueryHandler : IRequestHandler<GetModuleByIdQuery, ApiResult<ModuleDto>>
{
    private readonly IModuleRepository _moduleRepository;
    private readonly IStageRepository _stageRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetModuleByIdQueryHandler(
        IModuleRepository moduleRepository,
        IStageRepository stageRepository,
        IProductRepository productRepository,
        IMapper _mapper)
    {
        _moduleRepository = moduleRepository;
        _stageRepository = stageRepository;
        _productRepository = productRepository;
        this._mapper = _mapper;
    }

    public async Task<ApiResult<ModuleDto>> Handle(GetModuleByIdQuery request, CancellationToken cancellationToken)
    {
        // Get the module
        var module = await _moduleRepository.GetByIdAsync(ModuleId.From(request.ModuleId), cancellationToken);
        if (module == null)
        {
            return ApiResult<ModuleDto>.Fail(
                ErrorCodes.Module.NotFound,
                ErrorMessages.GetMessage(ErrorCodes.Module.NotFound),
                HttpStatusCodes.NotFound);
        }

        // Map to DTO
        var moduleDto = _mapper.Map<ModuleDto>(module);

        // Get stages
        var stages = await _stageRepository.GetStagesByModuleAsync(module.Id, cancellationToken);
        moduleDto = moduleDto with { StagesCount = stages.Count };

        // Get stages list
        var stageDtos = _mapper.Map<List<StageListDto>>(stages);
        moduleDto = moduleDto with { Stages = stageDtos };

        // Get price if product exists
        if (module.ProductId != null)
        {
            var product = await _productRepository.GetByIdAsync(module.ProductId, cancellationToken);
            moduleDto = moduleDto with { Price = product?.CashPrice };
        }

        return ApiResult<ModuleDto>.Ok(moduleDto);
    }
}
