using LMS.Domain.Common;

namespace LMS.Domain.Content;

/// <summary>
/// Represents a prerequisite rule that must be satisfied before accessing content
/// Supports complex prerequisite logic with grouping (AND/OR operators)
/// </summary>
public sealed class ContentPrerequisite : Entity<Guid>
{
    /// <summary>
    /// Content item that has this prerequisite
    /// </summary>
    public Guid ContentItemId { get; private set; }

    /// <summary>
    /// Type of prerequisite requirement
    /// </summary>
    public PrerequisiteType PrerequisiteType { get; private set; }

    /// <summary>
    /// Target content that must be completed (for CompleteContent type)
    /// </summary>
    public Guid? TargetContentId { get; private set; }

    /// <summary>
    /// Target stage that must be completed (for CompleteStage type)
    /// </summary>
    public Guid? TargetStageId { get; private set; }

    /// <summary>
    /// Target module that must be completed (for CompleteModule type)
    /// </summary>
    public Guid? TargetModuleId { get; private set; }

    /// <summary>
    /// Target assessment that must be passed (for PassAssessment type)
    /// </summary>
    public Guid? TargetAssessmentId { get; private set; }

    /// <summary>
    /// Required score for assessment prerequisites
    /// </summary>
    public short? RequiredScore { get; private set; }

    /// <summary>
    /// Number of days to delay access (for TimeDelay type)
    /// </summary>
    public short? DelayDays { get; private set; }

    /// <summary>
    /// Logical operator for grouping prerequisites (AND/OR)
    /// </summary>
    public GroupOperator GroupOperator { get; private set; }

    /// <summary>
    /// Group identifier for complex prerequisite rules
    /// Prerequisites with the same GroupId are evaluated together
    /// </summary>
    public short GroupId { get; private set; }

    /// <summary>
    /// Display order for UI presentation
    /// </summary>
    public short DisplayOrder { get; private set; }

    // EF Core constructor
    private ContentPrerequisite() : base()
    {
    }

    private ContentPrerequisite(
        Guid id,
        Guid contentItemId,
        PrerequisiteType prerequisiteType,
        Guid? targetContentId,
        Guid? targetStageId,
        Guid? targetModuleId,
        Guid? targetAssessmentId,
        short? requiredScore,
        short? delayDays,
        GroupOperator groupOperator,
        short groupId,
        short displayOrder)
        : base(id)
    {
        ContentItemId = contentItemId;
        PrerequisiteType = prerequisiteType;
        TargetContentId = targetContentId;
        TargetStageId = targetStageId;
        TargetModuleId = targetModuleId;
        TargetAssessmentId = targetAssessmentId;
        RequiredScore = requiredScore;
        DelayDays = delayDays;
        GroupOperator = groupOperator;
        GroupId = groupId;
        DisplayOrder = displayOrder;
    }

    /// <summary>
    /// Factory method to create a prerequisite for completing specific content
    /// </summary>
    public static Result<ContentPrerequisite> CreateForContent(
        Guid contentItemId,
        Guid targetContentId,
        GroupOperator groupOperator = GroupOperator.And,
        short groupId = 0,
        short displayOrder = 0)
    {
        if (contentItemId == Guid.Empty)
        {
            return Result<ContentPrerequisite>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (targetContentId == Guid.Empty)
        {
            return Result<ContentPrerequisite>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        var prerequisite = new ContentPrerequisite(
            Guid.NewGuid(),
            contentItemId,
            PrerequisiteType.CompleteContent,
            targetContentId,
            null,
            null,
            null,
            null,
            null,
            groupOperator,
            groupId,
            displayOrder
        );

        return Result<ContentPrerequisite>.Success(prerequisite);
    }

    /// <summary>
    /// Factory method to create a prerequisite for completing a stage
    /// </summary>
    public static Result<ContentPrerequisite> CreateForStage(
        Guid contentItemId,
        Guid targetStageId,
        GroupOperator groupOperator = GroupOperator.And,
        short groupId = 0,
        short displayOrder = 0)
    {
        if (contentItemId == Guid.Empty)
        {
            return Result<ContentPrerequisite>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (targetStageId == Guid.Empty)
        {
            return Result<ContentPrerequisite>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        var prerequisite = new ContentPrerequisite(
            Guid.NewGuid(),
            contentItemId,
            PrerequisiteType.CompleteStage,
            null,
            targetStageId,
            null,
            null,
            null,
            null,
            groupOperator,
            groupId,
            displayOrder
        );

        return Result<ContentPrerequisite>.Success(prerequisite);
    }

    /// <summary>
    /// Factory method to create a prerequisite for completing a module
    /// </summary>
    public static Result<ContentPrerequisite> CreateForModule(
        Guid contentItemId,
        Guid targetModuleId,
        GroupOperator groupOperator = GroupOperator.And,
        short groupId = 0,
        short displayOrder = 0)
    {
        if (contentItemId == Guid.Empty)
        {
            return Result<ContentPrerequisite>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (targetModuleId == Guid.Empty)
        {
            return Result<ContentPrerequisite>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        var prerequisite = new ContentPrerequisite(
            Guid.NewGuid(),
            contentItemId,
            PrerequisiteType.CompleteModule,
            null,
            null,
            targetModuleId,
            null,
            null,
            null,
            groupOperator,
            groupId,
            displayOrder
        );

        return Result<ContentPrerequisite>.Success(prerequisite);
    }

    /// <summary>
    /// Factory method to create a prerequisite for passing an assessment
    /// </summary>
    public static Result<ContentPrerequisite> CreateForAssessment(
        Guid contentItemId,
        Guid targetAssessmentId,
        short requiredScore,
        GroupOperator groupOperator = GroupOperator.And,
        short groupId = 0,
        short displayOrder = 0)
    {
        if (contentItemId == Guid.Empty)
        {
            return Result<ContentPrerequisite>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (targetAssessmentId == Guid.Empty)
        {
            return Result<ContentPrerequisite>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (requiredScore < 0 || requiredScore > 100)
        {
            return Result<ContentPrerequisite>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        var prerequisite = new ContentPrerequisite(
            Guid.NewGuid(),
            contentItemId,
            PrerequisiteType.PassAssessment,
            null,
            null,
            null,
            targetAssessmentId,
            requiredScore,
            null,
            groupOperator,
            groupId,
            displayOrder
        );

        return Result<ContentPrerequisite>.Success(prerequisite);
    }

    /// <summary>
    /// Factory method to create a prerequisite for follow-up approval
    /// </summary>
    public static Result<ContentPrerequisite> CreateForFollowUpApproval(
        Guid contentItemId,
        GroupOperator groupOperator = GroupOperator.And,
        short groupId = 0,
        short displayOrder = 0)
    {
        if (contentItemId == Guid.Empty)
        {
            return Result<ContentPrerequisite>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        var prerequisite = new ContentPrerequisite(
            Guid.NewGuid(),
            contentItemId,
            PrerequisiteType.FollowUpApproval,
            null,
            null,
            null,
            null,
            null,
            null,
            groupOperator,
            groupId,
            displayOrder
        );

        return Result<ContentPrerequisite>.Success(prerequisite);
    }

    /// <summary>
    /// Factory method to create a prerequisite for purchase
    /// </summary>
    public static Result<ContentPrerequisite> CreateForPurchase(
        Guid contentItemId,
        GroupOperator groupOperator = GroupOperator.And,
        short groupId = 0,
        short displayOrder = 0)
    {
        if (contentItemId == Guid.Empty)
        {
            return Result<ContentPrerequisite>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        var prerequisite = new ContentPrerequisite(
            Guid.NewGuid(),
            contentItemId,
            PrerequisiteType.Purchase,
            null,
            null,
            null,
            null,
            null,
            null,
            groupOperator,
            groupId,
            displayOrder
        );

        return Result<ContentPrerequisite>.Success(prerequisite);
    }

    /// <summary>
    /// Factory method to create a time delay prerequisite
    /// </summary>
    public static Result<ContentPrerequisite> CreateForTimeDelay(
        Guid contentItemId,
        short delayDays,
        GroupOperator groupOperator = GroupOperator.And,
        short groupId = 0,
        short displayOrder = 0)
    {
        if (contentItemId == Guid.Empty)
        {
            return Result<ContentPrerequisite>.Failure(
                Error.Validation(ErrorCodes.Validation.Required)
            );
        }

        if (delayDays <= 0)
        {
            return Result<ContentPrerequisite>.Failure(
                Error.Validation(ErrorCodes.Validation.InvalidInput)
            );
        }

        var prerequisite = new ContentPrerequisite(
            Guid.NewGuid(),
            contentItemId,
            PrerequisiteType.TimeDelay,
            null,
            null,
            null,
            null,
            null,
            delayDays,
            groupOperator,
            groupId,
            displayOrder
        );

        return Result<ContentPrerequisite>.Success(prerequisite);
    }

    /// <summary>
    /// Updates the grouping configuration
    /// </summary>
    public void UpdateGrouping(GroupOperator groupOperator, short groupId, short displayOrder)
    {
        GroupOperator = groupOperator;
        GroupId = groupId;
        DisplayOrder = displayOrder;
    }
}
