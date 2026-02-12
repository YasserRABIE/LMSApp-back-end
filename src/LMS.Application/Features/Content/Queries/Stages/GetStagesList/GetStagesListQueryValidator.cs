using FluentValidation;
using LMS.Application.Common;
using LMS.Domain.Common;

namespace LMS.Application.Features.Content.Queries.Stages.GetStagesList;

/// <summary>
/// Validator for GetStagesListQuery
/// </summary>
public sealed class GetStagesListQueryValidator : AbstractValidator<GetStagesListQuery>
{
    public GetStagesListQueryValidator()
    {
        RuleFor(x => x.ModuleId)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.Required));
    }
}
