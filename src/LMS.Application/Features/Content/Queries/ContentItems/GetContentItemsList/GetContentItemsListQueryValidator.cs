using FluentValidation;
using LMS.Application.Common;
using LMS.Domain.Common;

namespace LMS.Application.Features.Content.Queries.ContentItems.GetContentItemsList;

/// <summary>
/// Validator for GetContentItemsListQuery
/// </summary>
public sealed class GetContentItemsListQueryValidator : AbstractValidator<GetContentItemsListQuery>
{
    public GetContentItemsListQueryValidator()
    {
        RuleFor(x => x.StageId)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.Required));
    }
}
