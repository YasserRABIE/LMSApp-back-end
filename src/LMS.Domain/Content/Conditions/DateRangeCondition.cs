namespace LMS.Domain.Content.Conditions;

/// <summary>
/// Content is only available within a specific date range
/// </summary>
public sealed class DateRangeCondition : PrerequisiteCondition
{
    public override ConditionType ConditionType => ConditionType.DateRange;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    private DateRangeCondition() : base()
    {
    }

    private DateRangeCondition(
        ConditionId id,
        PrerequisiteId prerequisiteId,
        DateTime startDate,
        DateTime endDate,
        int displayOrder) : base(id, prerequisiteId, displayOrder)
    {
        StartDate = startDate;
        EndDate = endDate;
    }

    public static DateRangeCondition Create(
        PrerequisiteId prerequisiteId,
        DateTime startDate,
        DateTime endDate,
        int displayOrder = 0)
    {
        return new DateRangeCondition(
            ConditionId.New(),
            prerequisiteId,
            startDate,
            endDate,
            displayOrder);
    }
}
