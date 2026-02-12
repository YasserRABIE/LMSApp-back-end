namespace LMS.Application.Common;

public static class ErrorMessages
{
    private static readonly Dictionary<string, string> Messages = new()
    {
        // ===== USER ERRORS =====
        [Domain.Common.ErrorCodes.User.NotFound] = "لم يتم العثور على المستخدم",
        [Domain.Common.ErrorCodes.User.PhoneAlreadyExists] = "رقم الهاتف مسجل بالفعل",
        [Domain.Common.ErrorCodes.User.InvalidPhone] = "صيغة رقم الهاتف غير صحيحة. الصيغة المتوقعة: 01XXXXXXXXX",
        [Domain.Common.ErrorCodes.User.InvalidOtp] = "رمز التحقق غير صحيح",
        [Domain.Common.ErrorCodes.User.OtpExpired] = "انتهت صلاحية رمز التحقق",
        [Domain.Common.ErrorCodes.User.OtpRateLimitExceeded] = "تم تجاوز الحد الأقصى لمحاولات إرسال رمز التحقق. حاول مرة أخرى بعد {0} دقيقة",
        [Domain.Common.ErrorCodes.User.AlreadyVerified] = "تم التحقق من رقم الهاتف بالفعل",
        [Domain.Common.ErrorCodes.User.NotVerified] = "لم يتم التحقق من رقم الهاتف",
        [Domain.Common.ErrorCodes.User.Inactive] = "الحساب غير نشط",

        // ===== AUTH ERRORS =====
        [Domain.Common.ErrorCodes.Auth.InvalidCredentials] = "رقم الهاتف أو كلمة المرور غير صحيحة",
        [Domain.Common.ErrorCodes.Auth.TokenExpired] = "انتهت صلاحية الرمز",
        [Domain.Common.ErrorCodes.Auth.InvalidToken] = "الرمز غير صالح",
        [Domain.Common.ErrorCodes.Auth.RefreshTokenExpired] = "انتهت صلاحية رمز التحديث",
        [Domain.Common.ErrorCodes.Auth.InvalidRefreshToken] = "رمز التحديث غير صالح",
        [Domain.Common.ErrorCodes.Auth.SessionNotFound] = "لم يتم العثور على الجلسة",
        [Domain.Common.ErrorCodes.Auth.SessionRevoked] = "تم إلغاء الجلسة",
        [Domain.Common.ErrorCodes.Auth.InvalidVerificationToken] = "رمز التحقق غير صالح",
        [Domain.Common.ErrorCodes.Auth.MaxDevicesReached] = "تم الوصول للحد الأقصى من الأجهزة المسموح بها",

        // ===== TAG ERRORS =====
        [Domain.Common.ErrorCodes.Tag.NameRequired] = "اسم الوسم مطلوب",
        [Domain.Common.ErrorCodes.Tag.NameTooLong] = "اسم الوسم لا يمكن أن يتجاوز 50 حرف",
        [Domain.Common.ErrorCodes.Tag.NotFound] = "لم يتم العثور على الوسم",
        [Domain.Common.ErrorCodes.Tag.AlreadyExists] = "الوسم موجود بالفعل",

        // ===== SUBJECT ERRORS =====
        [Domain.Common.ErrorCodes.Subject.NameRequired] = "اسم المادة مطلوب",
        [Domain.Common.ErrorCodes.Subject.NameTooLong] = "اسم المادة لا يمكن أن يتجاوز 100 حرف",
        [Domain.Common.ErrorCodes.Subject.IconRequired] = "أيقونة المادة مطلوبة",
        [Domain.Common.ErrorCodes.Subject.ColorRequired] = "لون المادة مطلوب",
        [Domain.Common.ErrorCodes.Subject.InvalidDisplayOrder] = "ترتيب العرض غير صالح",
        [Domain.Common.ErrorCodes.Subject.NotFound] = "لم يتم العثور على المادة",

        // ===== SCHOOL TYPE ERRORS =====
        [Domain.Common.ErrorCodes.SchoolType.NameRequired] = "اسم نوع المدرسة مطلوب",
        [Domain.Common.ErrorCodes.SchoolType.NameTooLong] = "اسم نوع المدرسة لا يمكن أن يتجاوز 50 حرف",
        [Domain.Common.ErrorCodes.SchoolType.NotFound] = "لم يتم العثور على نوع المدرسة",

        // ===== COURSE CATEGORY ERRORS =====
        [Domain.Common.ErrorCodes.CourseCategory.NameRequired] = "اسم فئة الكورس مطلوب",
        [Domain.Common.ErrorCodes.CourseCategory.NameTooLong] = "اسم فئة الكورس لا يمكن أن يتجاوز 50 حرف",
        [Domain.Common.ErrorCodes.CourseCategory.NotFound] = "لم يتم العثور على فئة الكورس",

        // ===== COURSE ERRORS =====
        [Domain.Common.ErrorCodes.Course.NotFound] = "لم يتم العثور على الكورس",
        [Domain.Common.ErrorCodes.Course.AlreadyPublished] = "الكورس منشور بالفعل",
        [Domain.Common.ErrorCodes.Course.NotPublished] = "الكورس غير منشور",
        [Domain.Common.ErrorCodes.Course.Unauthorized] = "غير مصرح لك بالوصول لهذا الكورس",
        [Domain.Common.ErrorCodes.Course.TitleRequired] = "عنوان الكورس مطلوب",
        [Domain.Common.ErrorCodes.Course.TitleTooLong] = "عنوان الكورس لا يمكن أن يتجاوز 300 حرف",
        [Domain.Common.ErrorCodes.Course.DescriptionRequired] = "وصف الكورس مطلوب",
        [Domain.Common.ErrorCodes.Course.ThumbnailRequired] = "صورة الكورس مطلوبة",
        [Domain.Common.ErrorCodes.Course.InvalidStudyLevel] = "المستوى الدراسي غير صالح",
        [Domain.Common.ErrorCodes.Course.InvalidTrack] = "المسار الدراسي غير صالح",

        // ===== MODULE ERRORS =====
        [Domain.Common.ErrorCodes.Module.NotFound] = "لم يتم العثور على الوحدة",
        [Domain.Common.ErrorCodes.Module.NotAccessible] = "الوحدة غير متاحة",
        [Domain.Common.ErrorCodes.Module.NotEnrolled] = "لم تقم بالتسجيل في هذه الوحدة",
        [Domain.Common.ErrorCodes.Module.TitleRequired] = "عنوان الوحدة مطلوب",
        [Domain.Common.ErrorCodes.Module.TitleTooLong] = "عنوان الوحدة لا يمكن أن يتجاوز 300 حرف",
        [Domain.Common.ErrorCodes.Module.InvalidDisplayOrder] = "ترتيب العرض غير صالح",
        [Domain.Common.ErrorCodes.Module.InvalidEstimatedHours] = "عدد الساعات المقدرة غير صالح",

        // ===== STAGE ERRORS =====
        [Domain.Common.ErrorCodes.Stage.NotFound] = "لم يتم العثور على المرحلة",
        [Domain.Common.ErrorCodes.Stage.TitleRequired] = "عنوان المرحلة مطلوب",
        [Domain.Common.ErrorCodes.Stage.TitleTooLong] = "عنوان المرحلة لا يمكن أن يتجاوز 300 حرف",
        [Domain.Common.ErrorCodes.Stage.InvalidDisplayOrder] = "ترتيب العرض غير صالح",

        // ===== CONTENT ERRORS =====
        [Domain.Common.ErrorCodes.Content.NotFound] = "لم يتم العثور على المحتوى",
        [Domain.Common.ErrorCodes.Content.PrerequisiteNotMet] = "يجب إكمال المحتوى المطلوب أولاً",
        [Domain.Common.ErrorCodes.Content.NotAccessible] = "المحتوى غير متاح",
        [Domain.Common.ErrorCodes.Content.InvalidContentType] = "نوع المحتوى غير صالح",
        [Domain.Common.ErrorCodes.Content.TitleRequired] = "عنوان المحتوى مطلوب",
        [Domain.Common.ErrorCodes.Content.TitleTooLong] = "عنوان المحتوى لا يمكن أن يتجاوز 300 حرف",
        [Domain.Common.ErrorCodes.Content.InvalidDisplayOrder] = "ترتيب العرض غير صالح",
        [Domain.Common.ErrorCodes.Content.InvalidXpReward] = "نقاط الخبرة غير صالحة",
        [Domain.Common.ErrorCodes.Content.InvalidPurchasingPointsReward] = "نقاط الشراء غير صالحة",
        [Domain.Common.ErrorCodes.Content.FileNameRequired] = "اسم الملف مطلوب",
        [Domain.Common.ErrorCodes.Content.FileNameTooLong] = "اسم الملف لا يمكن أن يتجاوز 300 حرف",
        [Domain.Common.ErrorCodes.Content.FileUrlRequired] = "رابط الملف مطلوب",
        [Domain.Common.ErrorCodes.Content.InvalidFileSize] = "حجم الملف غير صالح",
        [Domain.Common.ErrorCodes.Content.MimeTypeRequired] = "نوع الملف مطلوب",
        [Domain.Common.ErrorCodes.Content.MimeTypeTooLong] = "نوع الملف لا يمكن أن يتجاوز 100 حرف",
        [Domain.Common.ErrorCodes.Content.ProviderIdRequired] = "معرّف مزود الخدمة مطلوب",
        [Domain.Common.ErrorCodes.Content.ProviderIdTooLong] = "معرّف مزود الخدمة لا يمكن أن يتجاوز 50 حرف",
        [Domain.Common.ErrorCodes.Content.ExternalVideoIdRequired] = "معرّف الفيديو الخارجي مطلوب",
        [Domain.Common.ErrorCodes.Content.ExternalVideoIdTooLong] = "معرّف الفيديو الخارجي لا يمكن أن يتجاوز 500 حرف",
        [Domain.Common.ErrorCodes.Content.InvalidDuration] = "مدة الفيديو غير صالحة",
        [Domain.Common.ErrorCodes.Content.VideoAlreadyReady] = "الفيديو جاهز بالفعل",
        [Domain.Common.ErrorCodes.Content.StorageProviderRequired] = "مزود التخزين مطلوب",
        [Domain.Common.ErrorCodes.Content.StorageProviderTooLong] = "مزود التخزين لا يمكن أن يتجاوز 20 حرف",
        [Domain.Common.ErrorCodes.Content.StoragePathRequired] = "مسار التخزين مطلوب",
        [Domain.Common.ErrorCodes.Content.StoragePathTooLong] = "مسار التخزين لا يمكن أن يتجاوز 1000 حرف",
        [Domain.Common.ErrorCodes.Content.FileExtensionRequired] = "امتداد الملف مطلوب",
        [Domain.Common.ErrorCodes.Content.FileExtensionTooLong] = "امتداد الملف لا يمكن أن يتجاوز 20 حرف",

        // ===== PRODUCT ERRORS =====
        [Domain.Common.ErrorCodes.Product.NotFound] = "لم يتم العثور على المنتج",
        [Domain.Common.ErrorCodes.Product.InvalidPrice] = "السعر غير صالح",
        [Domain.Common.ErrorCodes.Product.InvalidDiscount] = "نسبة الخصم غير صالحة",
        [Domain.Common.ErrorCodes.Product.InvalidPurchasingPoints] = "نقاط الشراء غير صالحة",
        [Domain.Common.ErrorCodes.Product.NoPriceSpecified] = "يجب تحديد سعر نقدي أو نقاط شراء",
        [Domain.Common.ErrorCodes.Product.InvalidCashPrice] = "السعر النقدي غير صالح",
        [Domain.Common.ErrorCodes.Product.InvalidPointsPrice] = "سعر النقاط غير صالح",
        [Domain.Common.ErrorCodes.Product.InvalidPointsReward] = "مكافأة النقاط غير صالحة",
        [Domain.Common.ErrorCodes.Product.InvalidDiscountPercentage] = "نسبة الخصم يجب أن تكون بين 0 و 100",
        [Domain.Common.ErrorCodes.Product.InvalidReferenceId] = "معرّف المرجع غير صالح",

        // ===== ASSESSMENT ERRORS =====
        [Domain.Common.ErrorCodes.Assessment.NotFound] = "لم يتم العثور على التقييم",
        [Domain.Common.ErrorCodes.Assessment.MaxRetakesExceeded] = "تم تجاوز الحد الأقصى لعدد المحاولات",
        [Domain.Common.ErrorCodes.Assessment.AlreadySubmitted] = "تم تسليم الإجابة بالفعل",
        [Domain.Common.ErrorCodes.Assessment.NotStarted] = "لم يتم بدء التقييم بعد",
        [Domain.Common.ErrorCodes.Assessment.TimeExpired] = "انتهى وقت التقييم",
        [Domain.Common.ErrorCodes.Assessment.InvalidAttempt] = "محاولة غير صالحة",

        // ===== PREREQUISITE ERRORS =====
        [Domain.Common.ErrorCodes.Prerequisite.NotFound] = "لم يتم العثور على الشرط المسبق",
        [Domain.Common.ErrorCodes.Prerequisite.NameRequired] = "اسم الشرط المسبق مطلوب",
        [Domain.Common.ErrorCodes.Prerequisite.NameTooLong] = "اسم الشرط المسبق لا يمكن أن يتجاوز 200 حرف",
        [Domain.Common.ErrorCodes.Prerequisite.InvalidTargetType] = "نوع الهدف غير صالح",
        [Domain.Common.ErrorCodes.Prerequisite.InvalidLogicOperator] = "المعامل المنطقي غير صالح",
        [Domain.Common.ErrorCodes.Prerequisite.NoConditions] = "يجب إضافة شرط واحد على الأقل",
        [Domain.Common.ErrorCodes.Prerequisite.NoActions] = "يجب إضافة إجراء واحد على الأقل",
        [Domain.Common.ErrorCodes.Prerequisite.InvalidConditionType] = "نوع الشرط غير صالح",
        [Domain.Common.ErrorCodes.Prerequisite.InvalidActionType] = "نوع الإجراء غير صالح",
        [Domain.Common.ErrorCodes.Prerequisite.InvalidConditionParameters] = "معاملات الشرط غير صالحة",
        [Domain.Common.ErrorCodes.Prerequisite.InvalidActionParameters] = "معاملات الإجراء غير صالحة",

        // ===== STUDY PLAN ERRORS =====
        [Domain.Common.ErrorCodes.StudyPlan.NotFound] = "لم يتم العثور على الخطة الدراسية",
        [Domain.Common.ErrorCodes.StudyPlan.NoSettings] = "لم يتم تعيين إعدادات الخطة الدراسية",
        [Domain.Common.ErrorCodes.StudyPlan.InvalidDateRange] = "نطاق التاريخ غير صالح",
        [Domain.Common.ErrorCodes.StudyPlan.AlreadyExists] = "الخطة الدراسية موجودة بالفعل",

        // ===== FOLLOW-UP ERRORS =====
        [Domain.Common.ErrorCodes.FollowUp.GroupNotFound] = "لم يتم العثور على المجموعة",
        [Domain.Common.ErrorCodes.FollowUp.GroupFull] = "المجموعة ممتلئة",
        [Domain.Common.ErrorCodes.FollowUp.NotEnrolled] = "لم تقم بالتسجيل في المتابعة",
        [Domain.Common.ErrorCodes.FollowUp.SessionNotFound] = "لم يتم العثور على الجلسة",
        [Domain.Common.ErrorCodes.FollowUp.SessionNotStarted] = "الجلسة لم تبدأ بعد",
        [Domain.Common.ErrorCodes.FollowUp.SessionAlreadyEnded] = "الجلسة انتهت بالفعل",

        // ===== GAMIFICATION ERRORS =====
        [Domain.Common.ErrorCodes.Gamification.InsufficientPoints] = "النقاط غير كافية",
        [Domain.Common.ErrorCodes.Gamification.InsufficientXp] = "نقاط الخبرة غير كافية",
        [Domain.Common.ErrorCodes.Gamification.NoStreakFreezeAvailable] = "لا يوجد تجميد متاح للسلسلة",
        [Domain.Common.ErrorCodes.Gamification.AchievementNotFound] = "لم يتم العثور على الإنجاز",
        [Domain.Common.ErrorCodes.Gamification.AchievementAlreadyUnlocked] = "تم فتح هذا الإنجاز بالفعل",

        // ===== ORDER/PAYMENT ERRORS =====
        [Domain.Common.ErrorCodes.Order.NotFound] = "لم يتم العثور على الطلب",
        [Domain.Common.ErrorCodes.Order.PaymentFailed] = "فشلت عملية الدفع",
        [Domain.Common.ErrorCodes.Order.AlreadyPaid] = "تم الدفع بالفعل",
        [Domain.Common.ErrorCodes.Order.Expired] = "انتهت صلاحية الطلب",
        [Domain.Common.ErrorCodes.Order.InvalidAmount] = "المبلغ غير صالح",

        // ===== PROMO CODE ERRORS =====
        [Domain.Common.ErrorCodes.PromoCode.NotFound] = "كود الخصم غير موجود",
        [Domain.Common.ErrorCodes.PromoCode.Expired] = "انتهت صلاحية كود الخصم",
        [Domain.Common.ErrorCodes.PromoCode.MaxUsesReached] = "تم الوصول للحد الأقصى لاستخدام كود الخصم",
        [Domain.Common.ErrorCodes.PromoCode.NotApplicable] = "كود الخصم غير قابل للتطبيق",
        [Domain.Common.ErrorCodes.PromoCode.AlreadyUsed] = "تم استخدام كود الخصم من قبل",

        // ===== WHATSAPP ERRORS =====
        [Domain.Common.ErrorCodes.WhatsApp.SendFailed] = "فشل إرسال رسالة الواتساب",
        [Domain.Common.ErrorCodes.WhatsApp.InvalidRecipient] = "المستلم غير صالح",
        [Domain.Common.ErrorCodes.WhatsApp.RateLimitExceeded] = "تم تجاوز الحد الأقصى لإرسال الرسائل",
        [Domain.Common.ErrorCodes.WhatsApp.TemplateNotApproved] = "القالب غير معتمد",

        // ===== SMS ERRORS =====
        [Domain.Common.ErrorCodes.Sms.SendFailed] = "فشل إرسال الرسالة النصية",
        [Domain.Common.ErrorCodes.Sms.InvalidCredentials] = "بيانات الاعتماد غير صالحة",
        [Domain.Common.ErrorCodes.Sms.InsufficientBalance] = "الرصيد غير كافٍ",
        [Domain.Common.ErrorCodes.Sms.InvalidRecipient] = "المستلم غير صالح",
        [Domain.Common.ErrorCodes.Sms.RateLimitExceeded] = "تم تجاوز الحد الأقصى لإرسال الرسائل",

        // ===== VALIDATION ERRORS =====
        [Domain.Common.ErrorCodes.Validation.InvalidInput] = "المدخلات غير صالحة",
        [Domain.Common.ErrorCodes.Validation.Required] = "هذا الحقل مطلوب",
        [Domain.Common.ErrorCodes.Validation.InvalidFormat] = "الصيغة غير صالحة",
        [Domain.Common.ErrorCodes.Validation.OutOfRange] = "القيمة خارج النطاق المسموح",
        [Domain.Common.ErrorCodes.Validation.PhoneRequired] = "رقم الهاتف مطلوب",
        [Domain.Common.ErrorCodes.Validation.PhoneInvalidFormat] = "يجب أن يكون رقم الهاتف بالصيغة المصرية (01XXXXXXXXX)",
        [Domain.Common.ErrorCodes.Validation.PasswordRequired] = "كلمة المرور مطلوبة",
        [Domain.Common.ErrorCodes.Validation.PasswordMinLength] = "يجب أن تكون كلمة المرور 8 أحرف على الأقل",
        [Domain.Common.ErrorCodes.Validation.PasswordComplexity] = "يجب أن تحتوي كلمة المرور على حرف كبير وحرف صغير ورقم واحد على الأقل",
        [Domain.Common.ErrorCodes.Validation.FirstNameRequired] = "الاسم الأول مطلوب",
        [Domain.Common.ErrorCodes.Validation.FirstNameMaxLength] = "الاسم الأول لا يمكن أن يتجاوز 100 حرف",
        [Domain.Common.ErrorCodes.Validation.SecondNameRequired] = "الاسم الثاني مطلوب",
        [Domain.Common.ErrorCodes.Validation.SecondNameMaxLength] = "الاسم الثاني لا يمكن أن يتجاوز 100 حرف",
        [Domain.Common.ErrorCodes.Validation.LastNameRequired] = "اسم العائلة مطلوب",
        [Domain.Common.ErrorCodes.Validation.LastNameMaxLength] = "اسم العائلة لا يمكن أن يتجاوز 100 حرف",
        [Domain.Common.ErrorCodes.Validation.StudyLevelTrackRequired] = "يجب اختيار المستوى الدراسي والمسار",
        [Domain.Common.ErrorCodes.Validation.SchoolNameMaxLength] = "اسم المدرسة لا يمكن أن يتجاوز 200 حرف",
        [Domain.Common.ErrorCodes.Validation.GovernorateMaxLength] = "المحافظة لا يمكن أن تتجاوز 100 حرف",
        [Domain.Common.ErrorCodes.Validation.DeviceFingerprintRequired] = "معرّف الجهاز مطلوب",
        [Domain.Common.ErrorCodes.Validation.PlatformRequired] = "نظام التشغيل مطلوب",
        [Domain.Common.ErrorCodes.Validation.PlatformMaxLength] = "نظام التشغيل لا يمكن أن يتجاوز 50 حرف",
        [Domain.Common.ErrorCodes.Validation.RefreshTokenRequired] = "رمز التحديث مطلوب",
        [Domain.Common.ErrorCodes.Validation.OtpCodeRequired] = "رمز التحقق مطلوب",
        [Domain.Common.ErrorCodes.Validation.OtpCodeLength] = "رمز التحقق يجب أن يكون 6 أرقام",
        [Domain.Common.ErrorCodes.Validation.OtpCodeDigitsOnly] = "رمز التحقق يجب أن يحتوي على أرقام فقط",
        [Domain.Common.ErrorCodes.Validation.SessionIdRequired] = "معرّف الجلسة مطلوب",
        [Domain.Common.ErrorCodes.Validation.UserIdRequired] = "معرّف المستخدم مطلوب",

        // ===== GENERAL ERRORS =====
        [Domain.Common.ErrorCodes.General.ServerError] = "حدث خطأ في الخادم",
        [Domain.Common.ErrorCodes.General.NotFound] = "العنصر المطلوب غير موجود",
        [Domain.Common.ErrorCodes.General.Unauthorized] = "غير مصرح لك بهذا الإجراء",
        [Domain.Common.ErrorCodes.General.Forbidden] = "ممنوع الوصول",
    };

    public static string GetMessage(string code, params object[] formatArgs)
    {
        if (Messages.TryGetValue(code, out var message))
        {
            // Support parameterized messages
            return formatArgs.Length > 0 ? string.Format(message, formatArgs) : message;
        }

        // Fallback: return code if message not found
        return code;
    }

    public static bool HasMessage(string code)
    {
        return Messages.ContainsKey(code);
    }
}
