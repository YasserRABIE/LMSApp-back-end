namespace LMS.Domain.Gamification;

public enum XpSource : byte { Lecture = 1, Quiz = 2, Assignment = 3, Challenge = 4, Streak = 5, FollowUp = 6, Achievement = 7, LevelUp = 8, Bonus = 9 }
public enum PointsTransactionType : byte { Earned = 1, Spent = 2, Purchased = 3, Refund = 4, Gift = 5, Promo = 6 }
public enum BenefitType : byte { Badge = 1, Discount = 2, Feature = 3, ExtraPoints = 4 }
public enum CriteriaType : byte { Streak = 1, QuizScore = 2, Completion = 3, SameDayCompletion = 4, ReviewOnTime = 5, NoMissedAssignments = 6, TopOfClass = 7, Custom = 99 }
