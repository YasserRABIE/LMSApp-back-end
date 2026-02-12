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

    // ===== TAG ERRORS =====
    public static class Tag
    {
        public const string NameRequired = "TAG.NAME_REQUIRED";
        public const string NameTooLong = "TAG.NAME_TOO_LONG";
        public const string NotFound = "TAG.NOT_FOUND";
        public const string AlreadyExists = "TAG.ALREADY_EXISTS";
    }

    // ===== SUBJECT ERRORS =====
    public static class Subject
    {
        public const string NameRequired = "SUBJECT.NAME_REQUIRED";
        public const string NameTooLong = "SUBJECT.NAME_TOO_LONG";
        public const string IconRequired = "SUBJECT.ICON_REQUIRED";
        public const string ColorRequired = "SUBJECT.COLOR_REQUIRED";
        public const string InvalidDisplayOrder = "SUBJECT.INVALID_DISPLAY_ORDER";
        public const string NotFound = "SUBJECT.NOT_FOUND";
    }

    // ===== SCHOOL TYPE ERRORS =====
    public static class SchoolType
    {
        public const string NameRequired = "SCHOOL_TYPE.NAME_REQUIRED";
        public const string NameTooLong = "SCHOOL_TYPE.NAME_TOO_LONG";
        public const string NotFound = "SCHOOL_TYPE.NOT_FOUND";
    }

    // ===== COURSE CATEGORY ERRORS =====
    public static class CourseCategory
    {
        public const string NameRequired = "COURSE_CATEGORY.NAME_REQUIRED";
        public const string NameTooLong = "COURSE_CATEGORY.NAME_TOO_LONG";
        public const string NotFound = "COURSE_CATEGORY.NOT_FOUND";
    }

    // ===== COURSE ERRORS =====
    public static class Course
    {
        public const string NotFound = "COURSE.NOT_FOUND";
        public const string AlreadyPublished = "COURSE.ALREADY_PUBLISHED";
        public const string NotPublished = "COURSE.NOT_PUBLISHED";
        public const string Unauthorized = "COURSE.UNAUTHORIZED";
        public const string TitleRequired = "COURSE.TITLE_REQUIRED";
        public const string TitleTooLong = "COURSE.TITLE_TOO_LONG";
        public const string DescriptionRequired = "COURSE.DESCRIPTION_REQUIRED";
        public const string ThumbnailRequired = "COURSE.THUMBNAIL_REQUIRED";
        public const string InvalidStudyLevel = "COURSE.INVALID_STUDY_LEVEL";
        public const string InvalidTrack = "COURSE.INVALID_TRACK";
    }

    // ===== MODULE ERRORS =====
    public static class Module
    {
        public const string NotFound = "MODULE.NOT_FOUND";
        public const string NotAccessible = "MODULE.NOT_ACCESSIBLE";
        public const string NotEnrolled = "MODULE.NOT_ENROLLED";
        public const string TitleRequired = "MODULE.TITLE_REQUIRED";
        public const string TitleTooLong = "MODULE.TITLE_TOO_LONG";
        public const string InvalidDisplayOrder = "MODULE.INVALID_DISPLAY_ORDER";
        public const string InvalidEstimatedHours = "MODULE.INVALID_ESTIMATED_HOURS";
    }

    // ===== STAGE ERRORS =====
    public static class Stage
    {
        public const string NotFound = "STAGE.NOT_FOUND";
        public const string TitleRequired = "STAGE.TITLE_REQUIRED";
        public const string TitleTooLong = "STAGE.TITLE_TOO_LONG";
        public const string InvalidDisplayOrder = "STAGE.INVALID_DISPLAY_ORDER";
    }

    // ===== CONTENT ERRORS =====
    public static class Content
    {
        public const string NotFound = "CONTENT.NOT_FOUND";
        public const string PrerequisiteNotMet = "CONTENT.PREREQUISITE_NOT_MET";
        public const string NotAccessible = "CONTENT.NOT_ACCESSIBLE";
        public const string InvalidContentType = "CONTENT.INVALID_CONTENT_TYPE";
        public const string TitleRequired = "CONTENT.TITLE_REQUIRED";
        public const string TitleTooLong = "CONTENT.TITLE_TOO_LONG";
        public const string InvalidDisplayOrder = "CONTENT.INVALID_DISPLAY_ORDER";
        public const string InvalidXpReward = "CONTENT.INVALID_XP_REWARD";
        public const string InvalidPurchasingPointsReward = "CONTENT.INVALID_PURCHASING_POINTS_REWARD";
        public const string FileNameRequired = "CONTENT.FILE_NAME_REQUIRED";
        public const string FileNameTooLong = "CONTENT.FILE_NAME_TOO_LONG";
        public const string FileUrlRequired = "CONTENT.FILE_URL_REQUIRED";
        public const string InvalidFileSize = "CONTENT.INVALID_FILE_SIZE";
        public const string MimeTypeRequired = "CONTENT.MIME_TYPE_REQUIRED";
        public const string MimeTypeTooLong = "CONTENT.MIME_TYPE_TOO_LONG";
        public const string ProviderIdRequired = "CONTENT.PROVIDER_ID_REQUIRED";
        public const string ProviderIdTooLong = "CONTENT.PROVIDER_ID_TOO_LONG";
        public const string ExternalVideoIdRequired = "CONTENT.EXTERNAL_VIDEO_ID_REQUIRED";
        public const string ExternalVideoIdTooLong = "CONTENT.EXTERNAL_VIDEO_ID_TOO_LONG";
        public const string InvalidDuration = "CONTENT.INVALID_DURATION";
        public const string VideoAlreadyReady = "CONTENT.VIDEO_ALREADY_READY";
        public const string StorageProviderRequired = "CONTENT.STORAGE_PROVIDER_REQUIRED";
        public const string StorageProviderTooLong = "CONTENT.STORAGE_PROVIDER_TOO_LONG";
        public const string StoragePathRequired = "CONTENT.STORAGE_PATH_REQUIRED";
        public const string StoragePathTooLong = "CONTENT.STORAGE_PATH_TOO_LONG";
        public const string FileExtensionRequired = "CONTENT.FILE_EXTENSION_REQUIRED";
        public const string FileExtensionTooLong = "CONTENT.FILE_EXTENSION_TOO_LONG";
    }

    // ===== PRODUCT ERRORS =====
    public static class Product
    {
        public const string NotFound = "PRODUCT.NOT_FOUND";
        public const string InvalidPrice = "PRODUCT.INVALID_PRICE";
        public const string InvalidDiscount = "PRODUCT.INVALID_DISCOUNT";
        public const string InvalidPurchasingPoints = "PRODUCT.INVALID_PURCHASING_POINTS";
        public const string NoPriceSpecified = "PRODUCT.NO_PRICE_SPECIFIED";
        public const string InvalidCashPrice = "PRODUCT.INVALID_CASH_PRICE";
        public const string InvalidPointsPrice = "PRODUCT.INVALID_POINTS_PRICE";
        public const string InvalidPointsReward = "PRODUCT.INVALID_POINTS_REWARD";
        public const string InvalidDiscountPercentage = "PRODUCT.INVALID_DISCOUNT_PERCENTAGE";
        public const string InvalidReferenceId = "PRODUCT.INVALID_REFERENCE_ID";
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

    // ===== PREREQUISITE ERRORS =====
    public static class Prerequisite
    {
        public const string NotFound = "PREREQUISITE.NOT_FOUND";
        public const string NameRequired = "PREREQUISITE.NAME_REQUIRED";
        public const string NameTooLong = "PREREQUISITE.NAME_TOO_LONG";
        public const string InvalidTargetType = "PREREQUISITE.INVALID_TARGET_TYPE";
        public const string InvalidLogicOperator = "PREREQUISITE.INVALID_LOGIC_OPERATOR";
        public const string NoConditions = "PREREQUISITE.NO_CONDITIONS";
        public const string NoActions = "PREREQUISITE.NO_ACTIONS";
        public const string InvalidConditionType = "PREREQUISITE.INVALID_CONDITION_TYPE";
        public const string InvalidActionType = "PREREQUISITE.INVALID_ACTION_TYPE";
        public const string InvalidConditionParameters = "PREREQUISITE.INVALID_CONDITION_PARAMETERS";
        public const string InvalidActionParameters = "PREREQUISITE.INVALID_ACTION_PARAMETERS";
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
