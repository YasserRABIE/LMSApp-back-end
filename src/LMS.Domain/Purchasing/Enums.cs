namespace LMS.Domain.Purchasing;

public enum ProductType : byte { Course = 1, Module = 2, Stage = 3, ContentItem = 4, PointsPackage = 5 }
public enum DiscountType : byte { Percentage = 1, FixedAmount = 2, FreePoints = 3 }
public enum OrderStatus : byte { Pending = 0, Processing = 1, Paid = 2, Failed = 3, Refunded = 4, PartiallyRefunded = 5, Cancelled = 6, Expired = 7 }
public enum PaymentMethod : byte { Card = 1, MobileWallet = 2, Fawry = 3, PointsOnly = 4, Mixed = 5 }
public enum EnrollmentStatus : byte { Active = 0, Paused = 1, Completed = 2, Expired = 3, Cancelled = 4 }
