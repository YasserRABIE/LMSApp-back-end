using FluentValidation;
using LMS.Application.Common;
using LMS.Domain.Common;

namespace LMS.Application.Features.Content.Queries.Modules.GetModulesList;

/// <summary>
/// Validator for GetModulesListQuery
/// </summary>
public sealed class GetModulesListQueryValidator : AbstractValidator<GetModulesListQuery>
{
    public GetModulesListQueryValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.Required));
    }
}
