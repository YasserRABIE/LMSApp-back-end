using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Domain.Common;
using LMS.Domain.Content;
using MediatR;

namespace LMS.Application.Features.Content.Commands.Modules.CreateModule;

public sealed class CreateModuleCommandHandler : IRequestHandler<CreateModuleCommand, ApiResult<Guid>>
{
    private readonly IModuleRepository _moduleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateModuleCommandHandler(
        IModuleRepository moduleRepository,
        IUnitOfWork unitOfWork)
    {
        _moduleRepository = moduleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult<Guid>> Handle(CreateModuleCommand request, CancellationToken cancellationToken)
    {
        // Create module using domain factory method
        var moduleResult = Module.Create(
            courseId: CourseId.From(request.CourseId),
            title: request.Title,
            description: request.Description,
            displayOrder: request.DisplayOrder,
            estimatedHours: request.EstimatedHours,
            thumbnail: request.Thumbnail);

        if (moduleResult.IsFailure)
        {
            var statusCode = moduleResult.Error.Type switch
            {
                ErrorType.Validation => HttpStatusCodes.BadRequest,
                ErrorType.NotFound => HttpStatusCodes.NotFound,
                ErrorType.Conflict => HttpStatusCodes.Conflict,
                ErrorType.Unauthorized => HttpStatusCodes.Unauthorized,
                ErrorType.Forbidden => HttpStatusCodes.Forbidden,
                _ => HttpStatusCodes.BadRequest
            };

            return ApiResult<Guid>.Fail(
                moduleResult.Error.Code,
                ErrorMessages.GetMessage(moduleResult.Error.Code),
                statusCode);
        }

        // Persist the module
        await _moduleRepository.AddAsync(moduleResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResult<Guid>.Ok(
            moduleResult.Value.Id.Value,
            SuccessMessages.ModuleCreated,
            HttpStatusCodes.Created);
    }
}
