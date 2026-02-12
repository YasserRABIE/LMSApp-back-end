using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Domain.Common;
using LMS.Domain.Content;
using MediatR;

namespace LMS.Application.Features.Content.Commands.Modules.DeleteModule;

public sealed class DeleteModuleCommandHandler : IRequestHandler<DeleteModuleCommand, ApiResult>
{
    private readonly IModuleRepository _moduleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteModuleCommandHandler(
        IModuleRepository moduleRepository,
        IUnitOfWork unitOfWork)
    {
        _moduleRepository = moduleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult> Handle(DeleteModuleCommand request, CancellationToken cancellationToken)
    {
        // Get the module
        var module = await _moduleRepository.GetByIdAsync(ModuleId.From(request.ModuleId), cancellationToken);
        if (module == null)
        {
            return ApiResult.Fail(
                ErrorCodes.Module.NotFound,
                ErrorMessages.GetMessage(ErrorCodes.Module.NotFound),
                HttpStatusCodes.NotFound);
        }

        // Soft delete by deactivating
        module.Deactivate();

        // Persist changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResult.Ok(SuccessMessages.ModuleDeleted);
    }
}
