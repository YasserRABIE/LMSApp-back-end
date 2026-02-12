using FluentValidation;
using LMS.Application.Common;
using LMS.Domain.Common;

namespace LMS.Application.Features.Content.Queries.ContentItems.GetContentItemById;

/// <summary>
/// Validator for GetContentItemByIdQuery
/// </summary>
public sealed class GetContentItemByIdQueryValidator : AbstractValidator<GetContentItemByIdQuery>
{
    public GetContentItemByIdQueryValidator()
    {
        RuleFor(x => x.ContentItemId)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.Required));
    }
}
