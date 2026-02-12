using FluentValidation;
using LMS.Application.Common;
using LMS.Domain.Common;

namespace LMS.Application.Features.Content.Commands.ContentItems.CreateContentItem;

/// <summary>
/// Validator for CreateContentItemCommand
/// </summary>
public sealed class CreateContentItemCommandValidator : AbstractValidator<CreateContentItemCommand>
{
    public CreateContentItemCommandValidator()
    {
        RuleFor(x => x.StageId)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.Required));

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.Required));

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Content.TitleRequired))
            .MaximumLength(300)
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Content.TitleTooLong));

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Content.InvalidDisplayOrder));

        RuleFor(x => x.XpReward)
            .GreaterThanOrEqualTo(0)
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Content.InvalidXpReward));

        RuleFor(x => x.PurchasingPointsReward)
            .GreaterThanOrEqualTo(0)
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Content.InvalidPurchasingPointsReward));
    }
}
