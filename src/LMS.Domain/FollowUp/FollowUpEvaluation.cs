using LMS.Domain.Common;

namespace LMS.Domain.FollowUp;

public sealed class FollowUpEvaluation : Entity<Guid>
{
    public Guid SessionId { get; private set; }
    public short CommitmentScore { get; private set; }
    public short UnderstandingScore { get; private set; }
    public decimal OverallScore { get; private set; }
    public StudentStatus StudentStatus { get; private set; }
    public string? PrivateNotes { get; private set; }
    public string? ParentNotes { get; private set; }
    public RecommendedAction RecommendedAction { get; private set; }
    public bool UnlockNextStage { get; private set; }
    public Guid? NextStageId { get; private set; }

    private FollowUpEvaluation() : base() { }

    private FollowUpEvaluation(Guid id, Guid sessionId, short commitmentScore, short understandingScore, StudentStatus studentStatus, RecommendedAction recommendedAction)
        : base(id)
    {
        SessionId = sessionId;
        CommitmentScore = commitmentScore;
        UnderstandingScore = understandingScore;
        OverallScore = (commitmentScore + understandingScore) / 2.0m;
        StudentStatus = studentStatus;
        RecommendedAction = recommendedAction;
    }

    public static Result<FollowUpEvaluation> Create(Guid sessionId, short commitmentScore, short understandingScore, StudentStatus studentStatus, RecommendedAction recommendedAction, string? privateNotes = null, string? parentNotes = null, bool unlockNextStage = false, Guid? nextStageId = null)
    {
        if (sessionId == Guid.Empty)
            return Result<FollowUpEvaluation>.Failure(Error.Validation(ErrorCodes.Validation.Required));
        if (commitmentScore < 1 || commitmentScore > 5)
            return Result<FollowUpEvaluation>.Failure(Error.Validation(ErrorCodes.Validation.InvalidInput));
        if (understandingScore < 1 || understandingScore > 5)
            return Result<FollowUpEvaluation>.Failure(Error.Validation(ErrorCodes.Validation.InvalidInput));

        var evaluation = new FollowUpEvaluation(Guid.NewGuid(), sessionId, commitmentScore, understandingScore, studentStatus, recommendedAction)
        {
            PrivateNotes = privateNotes,
            ParentNotes = parentNotes,
            UnlockNextStage = unlockNextStage,
            NextStageId = nextStageId
        };
        return Result<FollowUpEvaluation>.Success(evaluation);
    }

    public void UpdateNotes(string? privateNotes, string? parentNotes) { PrivateNotes = privateNotes; ParentNotes = parentNotes; }
    public void SetNextStageUnlock(Guid nextStageId) { UnlockNextStage = true; NextStageId = nextStageId; }
}
