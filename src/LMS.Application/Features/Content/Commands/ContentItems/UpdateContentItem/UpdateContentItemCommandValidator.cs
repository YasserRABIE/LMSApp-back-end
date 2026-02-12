using FluentValidation;
using LMS.Application.Common;
using LMS.Domain.Common;

namespace LMS.Application.Features.Content.Commands.ContentItems.UpdateContentItem;

/// <summary>
/// Validator for UpdateContentItemCommand
/// </summary>
public sealed class UpdateContentItemCommandValidator : AbstractValidator<UpdateContentItemCommand>
{
    public UpdateContentItemCommandValidator()
    {
        RuleFor(x => x.ContentItemId)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.Required));

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Content.TitleRequired))
            .MaximumLength(300)
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Content.TitleTooLong));

        RuleFor(x => x.XpReward)
            .GreaterThanOrEqualTo(0)
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Content.InvalidXpReward));

        RuleFor(x => x.PurchasingPointsReward)
            .GreaterThanOrEqualTo(0)
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Content.InvalidPurchasingPointsReward));
    }
}
