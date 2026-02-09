namespace LMS.Domain.Common;

/// <summary>
/// Centralized error codes for the entire application
/// Format: {DOMAIN}.{ERROR_TYPE}
/// </summary>
public static class ErrorCodes
{
    // ===== USER ERRORS =====
    public static class User
    {
        public const string NotFound = "USER.NOT_FOUND";
        public const string PhoneAlreadyExists = "USER.PHONE_ALREADY_EXISTS";
        public const string InvalidPhone = "USER.INVALID_PHONE";
        public const string InvalidOtp = "USER.INVALID_OTP";
        public const string OtpExpired = "USER.OTP_EXPIRED";
        public const string OtpRateLimitExceeded = "USER.OTP_RATE_LIMIT_EXCEEDED";
        public const string AlreadyVerified = "USER.ALREADY_VERIFIED";
        public const string NotVerified = "USER.NOT_VERIFIED";
        public const string Inactive = "USER.INACTIVE";
    }

    // ===== AUTH ERRORS =====
    public static class Auth
    {
        public const string InvalidCredentials = "AUTH.INVALID_CREDENTIALS";
        public const string TokenExpired = "AUTH.TOKEN_EXPIRED";
        public const string InvalidToken = "AUTH.INVALID_TOKEN";
        public const string RefreshTokenExpired = "AUTH.REFRESH_TOKEN_EXPIRED";
        public const string InvalidRefreshToken = "AUTH.INVALID_REFRESH_TOKEN";
        public const string SessionNotFound = "AUTH.SESSION_NOT_FOUND";
        public const string SessionRevoked = "AUTH.SESSION_REVOKED";
        public const string InvalidVerificationToken = "AUTH.INVALID_VERIFICATION_TOKEN";
        public const string MaxDevicesReached = "AUTH.MAX_DEVICES_REACHED";
    }

    // ===== COURSE ERRORS =====
    public static class Course
    {
        public const string NotFound = "COURSE.NOT_FOUND";
        public const string AlreadyPublished = "COURSE.ALREADY_PUBLISHED";
        public const string NotPublished = "COURSE.NOT_PUBLISHED";
        public const string Unauthorized = "COURSE.UNAUTHORIZED";
    }

    // ===== MODULE ERRORS =====
    public static class Module
    {
        public const string NotFound = "MODULE.NOT_FOUND";
        public const string NotAccessible = "MODULE.NOT_ACCESSIBLE";
        public const string NotEnrolled = "MODULE.NOT_ENROLLED";
    }

    // ===== CONTENT ERRORS =====
    public static class Content
    {
        public const string NotFound = "CONTENT.NOT_FOUND";
        public const string PrerequisiteNotMet = "CONTENT.PREREQUISITE_NOT_MET";
        public const string NotAccessible = "CONTENT.NOT_ACCESSIBLE";
    }

    // ===== ASSESSMENT ERRORS =====
    public static class Assessment
    {
        public const string NotFound = "ASSESSMENT.NOT_FOUND";
        public const string MaxRetakesExceeded = "ASSESSMENT.MAX_RETAKES_EXCEEDED";
        public const string AlreadySubmitted = "ASSESSMENT.ALREADY_SUBMITTED";
        public const string NotStarted = "ASSESSMENT.NOT_STARTED";
        public const string TimeExpired = "ASSESSMENT.TIME_EXPIRED";
        public const string InvalidAttempt = "ASSESSMENT.INVALID_ATTEMPT";
    }

    // ===== STUDY PLAN ERRORS =====
    public static class StudyPlan
    {
        public const string NotFound = "STUDY_PLAN.NOT_FOUND";
        public const string NoSettings = "STUDY_PLAN.NO_SETTINGS";
        public const string InvalidDateRange = "STUDY_PLAN.INVALID_DATE_RANGE";
        public const string AlreadyExists = "STUDY_PLAN.ALREADY_EXISTS";
    }

    // ===== FOLLOW-UP ERRORS =====
    public static class FollowUp
    {
        public const string GroupNotFound = "FOLLOW_UP.GROUP_NOT_FOUND";
        public const string GroupFull = "FOLLOW_UP.GROUP_FULL";
        public const string NotEnrolled = "FOLLOW_UP.NOT_ENROLLED";
        public const string SessionNotFound = "FOLLOW_UP.SESSION_NOT_FOUND";
        public const string SessionNotStarted = "FOLLOW_UP.SESSION_NOT_STARTED";
        public const string SessionAlreadyEnded = "FOLLOW_UP.SESSION_ALREADY_ENDED";
    }

    // ===== GAMIFICATION ERRORS =====
    public static class Gamification
    {
        public const string InsufficientPoints = "GAMIFICATION.INSUFFICIENT_POINTS";
        public const string InsufficientXp = "GAMIFICATION.INSUFFICIENT_XP";
        public const string NoStreakFreezeAvailable = "GAMIFICATION.NO_STREAK_FREEZE_AVAILABLE";
        public const string AchievementNotFound = "GAMIFICATION.ACHIEVEMENT_NOT_FOUND";
        public const string AchievementAlreadyUnlocked = "GAMIFICATION.ACHIEVEMENT_ALREADY_UNLOCKED";
    }

    // ===== ORDER/PAYMENT ERRORS =====
    public static class Order
    {
        public const string NotFound = "ORDER.NOT_FOUND";
        public const string PaymentFailed = "ORDER.PAYMENT_FAILED";
        public const string AlreadyPaid = "ORDER.ALREADY_PAID";
        public const string Expired = "ORDER.EXPIRED";
        public const string InvalidAmount = "ORDER.INVALID_AMOUNT";
    }

    // ===== PROMO CODE ERRORS =====
    public static class PromoCode
    {
        public const string NotFound = "PROMO_CODE.NOT_FOUND";
        public const string Expired = "PROMO_CODE.EXPIRED";
        public const string MaxUsesReached = "PROMO_CODE.MAX_USES_REACHED";
        public const string NotApplicable = "PROMO_CODE.NOT_APPLICABLE";
        public const string AlreadyUsed = "PROMO_CODE.ALREADY_USED";
    }

    // ===== WHATSAPP ERRORS ===== (Reserved for future WhatsApp integration)
    public static class WhatsApp
    {
        public const string SendFailed = "WHATSAPP.SEND_FAILED";
        public const string InvalidRecipient = "WHATSAPP.INVALID_RECIPIENT";
        public const string RateLimitExceeded = "WHATSAPP.RATE_LIMIT_EXCEEDED";
        public const string TemplateNotApproved = "WHATSAPP.TEMPLATE_NOT_APPROVED";
    }

    // ===== SMS ERRORS =====
    public static class Sms
    {
        public const string SendFailed = "SMS.SEND_FAILED";
        public const string InvalidCredentials = "SMS.INVALID_CREDENTIALS";
        public const string InsufficientBalance = "SMS.INSUFFICIENT_BALANCE";
        public const string InvalidRecipient = "SMS.INVALID_RECIPIENT";
        public const string RateLimitExceeded = "SMS.RATE_LIMIT_EXCEEDED";
    }

    // ===== VALIDATION ERRORS =====
    public static class Validation
    {
        public const string InvalidInput = "VALIDATION.INVALID_INPUT";
        public const string Required = "VALIDATION.REQUIRED";
        public const string InvalidFormat = "VALIDATION.INVALID_FORMAT";
        public const string OutOfRange = "VALIDATION.OUT_OF_RANGE";

        // Field-specific validation
        public const string PhoneRequired = "VALIDATION.PHONE_REQUIRED";
        public const string PhoneInvalidFormat = "VALIDATION.PHONE_INVALID_FORMAT";
        public const string PasswordRequired = "VALIDATION.PASSWORD_REQUIRED";
        public const string PasswordMinLength = "VALIDATION.PASSWORD_MIN_LENGTH";
        public const string PasswordComplexity = "VALIDATION.PASSWORD_COMPLEXITY";
        public const string FirstNameRequired = "VALIDATION.FIRST_NAME_REQUIRED";
        public const string FirstNameMaxLength = "VALIDATION.FIRST_NAME_MAX_LENGTH";
        public const string SecondNameRequired = "VALIDATION.SECOND_NAME_REQUIRED";
        public const string SecondNameMaxLength = "VALIDATION.SECOND_NAME_MAX_LENGTH";
        public const string LastNameRequired = "VALIDATION.LAST_NAME_REQUIRED";
        public const string LastNameMaxLength = "VALIDATION.LAST_NAME_MAX_LENGTH";
        public const string StudyLevelTrackRequired = "VALIDATION.STUDY_LEVEL_TRACK_REQUIRED";
        public const string SchoolNameMaxLength = "VALIDATION.SCHOOL_NAME_MAX_LENGTH";
        public const string GovernorateMaxLength = "VALIDATION.GOVERNORATE_MAX_LENGTH";
        public const string DeviceFingerprintRequired = "VALIDATION.DEVICE_FINGERPRINT_REQUIRED";
        public const string PlatformRequired = "VALIDATION.PLATFORM_REQUIRED";
        public const string PlatformMaxLength = "VALIDATION.PLATFORM_MAX_LENGTH";
        public const string RefreshTokenRequired = "VALIDATION.REFRESH_TOKEN_REQUIRED";
        public const string OtpCodeRequired = "VALIDATION.OTP_CODE_REQUIRED";
        public const string OtpCodeLength = "VALIDATION.OTP_CODE_LENGTH";
        public const string OtpCodeDigitsOnly = "VALIDATION.OTP_CODE_DIGITS_ONLY";
        public const string SessionIdRequired = "VALIDATION.SESSION_ID_REQUIRED";
        public const string UserIdRequired = "VALIDATION.USER_ID_REQUIRED";
    }

    // ===== GENERAL ERRORS =====
    public static class General
    {
        public const string ServerError = "GENERAL.SERVER_ERROR";
        public const string NotFound = "GENERAL.NOT_FOUND";
        public const string Unauthorized = "GENERAL.UNAUTHORIZED";
        public const string Forbidden = "GENERAL.FORBIDDEN";
    }
}
