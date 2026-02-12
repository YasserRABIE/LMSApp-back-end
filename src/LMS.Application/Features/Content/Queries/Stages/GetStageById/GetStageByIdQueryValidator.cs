using FluentValidation;
using LMS.Application.Common;
using LMS.Domain.Common;

namespace LMS.Application.Features.Content.Queries.Stages.GetStageById;

/// <summary>
/// Validator for GetStageByIdQuery
/// </summary>
public sealed class GetStageByIdQueryValidator : AbstractValidator<GetStageByIdQuery>
{
    public GetStageByIdQueryValidator()
    {
        RuleFor(x => x.StageId)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.Required));
    }
}
