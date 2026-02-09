namespace LMS.Domain.Notification;

public enum NotificationType : byte { System = 1, Content = 2, Assessment = 3, FollowUp = 4, StudyPlan = 5, Achievement = 6, Order = 7, Reminder = 8 }
public enum Priority : byte { Low = 1, Normal = 2, High = 3 }
public enum DeliveryChannel : byte { Push = 1, Email = 2, Sms = 3, InApp = 4 }
public enum DeliveryStatus : byte { Pending = 0, Sent = 1, Delivered = 2, Failed = 3 }
public enum Platform : byte { iOS = 1, Android = 2, Web = 3 }
