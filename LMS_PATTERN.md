# Egyptian LMS Implementation Patterns & Standards

**Purpose**: Defines architectural patterns, coding standards, and implementation workflow for the Egyptian LMS platform. Follow these patterns for consistency, maintainability, and quality.

**Last Updated**: February 9, 2026
**Based On**: DOZ ride-sharing architecture + LMS-specific requirements + Arabic-first localization

---

## Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [Layer Responsibilities](#layer-responsibilities)
3. [Localization & Error Handling](#localization--error-handling)
4. [Implementation Workflow](#implementation-workflow)
5. [Complete Examples](#complete-examples)
6. [Common Pitfalls](#common-pitfalls)
7. [Quick Checklist](#quick-checklist)

---

## Architecture Overview

```
┌─────────────────────────────────────────────────────────┐
│                     API Layer (ASP.NET)                 │
│  - Controllers (endpoints, auth)                        │
│  - Request/Response DTOs                                │
│  - ApiResult responses                                  │
└────────────────────┬────────────────────────────────────┘
                     │ Uses MediatR
                     ↓
┌─────────────────────────────────────────────────────────┐
│                Application Layer (CQRS)                 │
│  - Commands & Queries                                   │
│  - Handlers with ErrorMessages.GetMessage()            │
│  - FluentValidation (localized)                         │
│  - DTOs & Repository Interfaces                         │
└────────────────────┬────────────────────────────────────┘
                     │ Uses Domain + Repositories
                     ↓
┌─────────────────────────────────────────────────────────┐
│                   Domain Layer (DDD)                    │
│  - Aggregate Roots with factory methods                │
│  - Value Objects                                        │
│  - Error Codes (code-only, no messages)                │
│  - Business Rules                                       │
└────────────────────┬────────────────────────────────────┘
                     │ Implemented by
                     ↓
┌─────────────────────────────────────────────────────────┐
│              Infrastructure Layer                        │
│  - Repository Implementations                           │
│  - DbContext & EF Core                                  │
│  - External Services                                    │
└─────────────────────────────────────────────────────────┘
```

**Key Principles:**
- Dependencies flow inward: Infrastructure → Application → Domain
- Domain is isolated (no external dependencies)
- Result Pattern (no exceptions for business errors)
- Localization in Application layer (Arabic primary, English fallback)

---

## Layer Responsibilities

### Domain Layer (`LMS.Domain`)

**✅ Contains:**
- Aggregate Roots with private constructors + factory methods
- Value Objects (Phone, Email, Money)
- Error **Codes** only (e.g., `ErrorCodes.User.InvalidPhone`)
- Business rules & invariants
- Domain Events

**❌ Does NOT contain:**
- Error **Messages** (those belong in Application)
- Database concerns (EF Core)
- External service calls
- Application logic
- DTOs

**Pattern:**
```csharp
public sealed class Phone : ValueObject
{
    public string Value { get; private set; }
    private Phone(string value) => Value = value;

    public static Result<Phone> Create(string phone)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(phone))
            return Result<Phone>.Failure(
                Error.Validation(ErrorCodes.User.InvalidPhone));

        // Business logic
        var cleaned = phone.Replace(" ", "").Replace("-", "");
        if (!Regex.IsMatch(cleaned, @"^01[0125]\d{8}$"))
            return Result<Phone>.Failure(
                Error.Validation(ErrorCodes.User.InvalidPhone));

        return Result<Phone>.Success(new Phone(cleaned));
    }
}
```

---

### Application Layer (`LMS.Application`)

**✅ Contains:**
- Commands & Queries (MediatR)
- Handlers (with `ErrorMessages.GetMessage()` and `SuccessMessages`)
- FluentValidation validators (using `ErrorMessages.GetMessage()`)
- Repository interfaces
- DTOs for API responses
- **SuccessMessages** constants (Arabic) - NEW!
- **ErrorMessages** dictionary (Arabic-only)

**Pattern:**
```csharp
// Validator with localized messages
public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.PhoneRequired))
            .Matches(@"^01[0-9]{9}$")
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.PhoneInvalidFormat));
    }
}

// Handler with localized error & success messages
public async Task<ApiResult<UserDto>> Handle(...)
{
    // Validate
    var phoneResult = Phone.Create(request.Phone);
    if (phoneResult.IsFailure)
        return ApiResult<UserDto>.Fail(
            phoneResult.Error.Code,
            ErrorMessages.GetMessage(phoneResult.Error.Code),  // Arabic error
            HttpStatusCodes.BadRequest);

    // Business logic...
    var dto = new UserDto(...);

    // Return success with Arabic message
    return ApiResult<UserDto>.Ok(dto, SuccessMessages.RegistrationSuccess);
}
```

---

### Infrastructure Layer (`LMS.Infrastructure`)

**✅ Contains:**
- Repository implementations
- DbContext & entity configurations
- External service implementations (VdoCipher, SMS, etc.)

**Pattern:**
```csharp
public sealed class UserRepository : Repository<User, UserId>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context) { }

    public async Task<User?> GetByPhoneAsync(
        Phone phone,
        CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.StudentProfile)
            .FirstOrDefaultAsync(u => u.Phone == phone, cancellationToken);
    }
}
```

---

### API Layer (`LMS.API`)

**✅ Contains:**
- Controllers (thin - delegate to MediatR)
- Request DTOs
- Authorization
- Swagger documentation

**Pattern:**
```csharp
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    public AuthController(IMediator mediator) => _mediator = mediator;

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var command = new LoginCommand(
            request.Phone,
            request.Password,
            request.DeviceFingerprint,
            request.Platform);

        var result = await _mediator.Send(command);
        return StatusCode(result.StatusCode, result);
    }
}
```

---

## Localization & Error/Success Handling

### DOZ-Style Pattern (Clean Architecture Compliance)

**Philosophy**: Arabic-first for Egyptian users. Success and error messages are centralized in Application layer for consistency.

**1. Domain Layer - Error Codes Only**
```csharp
// Domain/Common/ErrorCodes.cs
public static class ErrorCodes
{
    public static class User
    {
        public const string NotFound = "USER.NOT_FOUND";
        public const string InvalidPhone = "USER.INVALID_PHONE";
        public const string PhoneAlreadyExists = "USER.PHONE_ALREADY_EXISTS";
    }

    public static class Validation
    {
        public const string PhoneRequired = "VALIDATION.PHONE_REQUIRED";
        public const string PasswordRequired = "VALIDATION.PASSWORD_REQUIRED";
    }
}

// Domain/Common/Error.cs (NO MESSAGES!)
public sealed record Error
{
    public string Code { get; }
    public ErrorType Type { get; }
    public string? Description { get; }  // Technical only, NOT user-facing

    private Error(string code, ErrorType type, string? description = null)
    {
        Code = code;
        Type = type;
        Description = description;
    }

    public static Error Validation(string code, string? description = null)
        => new(code, ErrorType.Validation, description);
}

// Domain entities return code-only errors
public static Result<User> Create(...)
{
    if (string.IsNullOrWhiteSpace(firstName))
        return Result<User>.Failure(
            Error.Validation(ErrorCodes.User.FirstNameRequired));
}
```

**2. Application Layer - Success & Error Messages (Arabic-Only)**
```csharp
// Application/Common/SuccessMessages.cs (NEW!)
public static class SuccessMessages
{
    // Authentication & Registration
    public const string RegistrationSuccess = "تم التسجيل بنجاح";
    public const string LoginSuccess = "تم تسجيل الدخول بنجاح";
    public const string TokenRefreshed = "تم تحديث الرمز بنجاح";
    public const string LogoutSuccess = "تم تسجيل الخروج بنجاح";
    public const string LogoutAllDevicesSuccess = "تم تسجيل الخروج من جميع الأجهزة بنجاح";
    public const string OtpSent = "تم إرسال رمز التحقق بنجاح";
    public const string PhoneVerified = "تم التحقق من الهاتف بنجاح";

    // Course & Enrollment
    public const string CourseEnrolled = "تم التسجيل في الكورس بنجاح";
    public const string CourseCreated = "تم إنشاء الكورس بنجاح";
    public const string CourseUpdated = "تم تحديث الكورس بنجاح";

    // Content & Progress
    public const string ProgressUpdated = "تم تحديث التقدم بنجاح";
    public const string ContentCompleted = "تم إكمال المحتوى بنجاح";

    // Assessments
    public const string AssessmentSubmitted = "تم تسليم التقييم بنجاح";
    public const string AssessmentStarted = "تم بدء التقييم بنجاح";

    // Generic
    public const string OperationSuccess = "تمت العملية بنجاح";
    public const string DataSaved = "تم حفظ البيانات بنجاح";
}

// Application/Common/ErrorMessages.cs
public static class ErrorMessages
{
    // Arabic messages for Egyptian users (NO English fallback)
    private static readonly Dictionary<string, string> Messages = new()
    {
        [ErrorCodes.User.InvalidPhone] = "صيغة رقم الهاتف غير صحيحة. الصيغة المتوقعة: 01XXXXXXXXX",
        [ErrorCodes.User.PhoneAlreadyExists] = "رقم الهاتف مسجل بالفعل",
        [ErrorCodes.Validation.PhoneRequired] = "رقم الهاتف مطلوب",
        [ErrorCodes.Validation.PasswordRequired] = "كلمة المرور مطلوبة",
        // ... all other error messages in Arabic
    };

    /// <summary>
    /// Gets error message in Arabic for the given code.
    /// Supports parameterized messages.
    /// </summary>
    public static string GetMessage(string code, params object[] formatArgs)
    {
        if (Messages.TryGetValue(code, out var message))
        {
            return formatArgs.Length > 0 ? string.Format(message, formatArgs) : message;
        }

        return code; // Fallback to code if message not found
    }

    public static bool HasMessage(string code)
    {
        return Messages.ContainsKey(code);
    }
}

// Handlers use ErrorMessages & SuccessMessages
public async Task<ApiResult<UserDto>> Handle(...)
{
    // Error case - use ErrorMessages
    if (phoneResult.IsFailure)
        return ApiResult<UserDto>.Fail(
            phoneResult.Error.Code,
            ErrorMessages.GetMessage(phoneResult.Error.Code),  // Arabic error
            HttpStatusCodes.BadRequest);

    // Success case - use SuccessMessages
    var dto = new UserDto(...);
    return ApiResult<UserDto>.Ok(dto, SuccessMessages.RegistrationSuccess);  // Arabic success
}

// FluentValidation uses ErrorMessages.GetMessage()
public sealed class RegisterStudentCommandValidator : AbstractValidator<RegisterStudentCommand>
{
    public RegisterStudentCommandValidator()
    {
        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.PhoneRequired));

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.PasswordRequired))
            .MinimumLength(8)
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.PasswordMinLength));
    }
}
```

**3. API Response Examples**

Success Response:
```json
{
  "data": {
    "id": "3aac7272-d69c-42e0-a41f-4eda19189ae5",
    "fullName": "Test User One",
    "phone": "01234567890",
    "userType": "Student"
  },
  "success": true,
  "error": null,
  "message": "تم التسجيل بنجاح",
  "statusCode": 201
}
```

Error Response:
```json
{
  "data": null,
  "success": false,
  "error": {
    "code": "VALIDATION.INVALID_INPUT",
    "message": "المدخلات غير صالحة",
    "validationErrors": {
      "Phone": ["يجب أن يكون رقم الهاتف بالصيغة المصرية (01XXXXXXXXX)"],
      "Password": ["كلمة المرور مطلوبة"]
    }
  },
  "message": null,
  "statusCode": 400
}
```

---

## Implementation Workflow

### For Each New Feature

**1. Domain Layer**
   - [ ] Create/update aggregate root with **private constructor**
   - [ ] Add **factory method** (e.g., `Create()`) with validation
   - [ ] Create value objects if needed
   - [ ] Define domain events if needed
   - [ ] Add error **codes** to `ErrorCodes.cs` (NO messages!)
   - [ ] Use `Error.Validation()`, `Error.NotFound()`, etc. in Result returns

**2. Application Layer**
   - [ ] Add error **messages** to `ErrorMessages.cs` (Arabic-only)
   - [ ] Add success **message** constant to `SuccessMessages.cs` (Arabic)
   - [ ] Create command/query record
   - [ ] Create **validator** using `ErrorMessages.GetMessage()`
   - [ ] Create handler
   - [ ] Use `ErrorMessages.GetMessage()` in all error responses
   - [ ] Use `SuccessMessages.XXX` in success responses
   - [ ] Create DTOs
   - [ ] Add repository interface if needed

**3. Infrastructure Layer**
   - [ ] Add entity configuration
   - [ ] Implement repository methods
   - [ ] Add migration if schema changed

**4. API Layer**
   - [ ] Create controller endpoint
   - [ ] Add request DTOs
   - [ ] Add authorization attributes
   - [ ] Document with XML comments
   - [ ] Return `StatusCode(result.StatusCode, result)`

**5. Testing**
   - [ ] Build succeeds (0 errors)
   - [ ] All tests pass
   - [ ] Test API with Postman/curl
   - [ ] Verify Arabic success messages in responses
   - [ ] Verify Arabic error messages
   - [ ] Verify validation errors in Arabic

---

## Complete Examples

### Command Example: RegisterStudentCommand

**Complete flow showing all layers with localization:**

#### 1. Domain - Phone Value Object
```csharp
// Domain/Common/Phone.cs
public sealed class Phone : ValueObject
{
    public string Value { get; }

    private Phone(string value) => Value = value;

    public static Result<Phone> Create(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return Result<Phone>.Failure(
                Error.Validation(ErrorCodes.User.InvalidPhone));

        // Egyptian phone format: 01XXXXXXXXX
        var cleaned = Regex.Replace(phone, @"[\s\-\(\)]", string.Empty);

        if (cleaned.StartsWith("+20"))
            cleaned = "0" + cleaned.Substring(3);

        if (!Regex.IsMatch(cleaned, @"^01[0125]\d{8}$"))
            return Result<Phone>.Failure(
                Error.Validation(ErrorCodes.User.InvalidPhone));

        return Result<Phone>.Success(new Phone(cleaned));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
```

#### 2. Domain - User Aggregate
```csharp
// Domain/Users/User.cs
public sealed class User : AggregateRoot<UserId>
{
    public UserId Id { get; private set; }
    public Phone Phone { get; private set; }
    public string PasswordHash { get; private set; }
    public string FirstName { get; private set; }
    public string SecondName { get; private set; }
    public string LastName { get; private set; }
    public UserType UserType { get; private set; }
    public bool IsPhoneVerified { get; private set; }
    public bool IsActive { get; private set; }

    private User() { }  // EF Core

    public static Result<User> Create(
        Phone phone,
        string passwordHash,
        string firstName,
        string secondName,
        string lastName,
        UserType userType,
        bool isPhoneVerified = false)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Result<User>.Failure(
                Error.Validation(ErrorCodes.User.FirstNameRequired));

        if (string.IsNullOrWhiteSpace(secondName))
            return Result<User>.Failure(
                Error.Validation(ErrorCodes.User.SecondNameRequired));

        if (string.IsNullOrWhiteSpace(lastName))
            return Result<User>.Failure(
                Error.Validation(ErrorCodes.User.LastNameRequired));

        if (string.IsNullOrWhiteSpace(passwordHash))
            return Result<User>.Failure(
                Error.Validation(ErrorCodes.User.PasswordRequired));

        var user = new User
        {
            Id = UserId.CreateUnique(),
            Phone = phone,
            PasswordHash = passwordHash,
            FirstName = firstName,
            SecondName = secondName,
            LastName = lastName,
            UserType = userType,
            IsPhoneVerified = isPhoneVerified,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        return Result<User>.Success(user);
    }

    public string GetFullName() => $"{FirstName} {SecondName} {LastName}";
}
```

#### 3. Application - Command & Validator
```csharp
// Application/Auth/Commands/RegisterStudent/RegisterStudentCommand.cs
public sealed record RegisterStudentCommand(
    string Phone,
    string Password,
    string FirstName,
    string SecondName,
    string LastName,
    Guid StudyLevelTrackId,
    string? SchoolName,
    string? Governorate
) : IRequest<ApiResult<UserInfoDto>>;

// Validator with localized messages
public sealed class RegisterStudentCommandValidator : AbstractValidator<RegisterStudentCommand>
{
    public RegisterStudentCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.FirstNameRequired))
            .MaximumLength(100)
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.FirstNameMaxLength));

        RuleFor(x => x.SecondName)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.SecondNameRequired))
            .MaximumLength(100)
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.SecondNameMaxLength));

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.LastNameRequired))
            .MaximumLength(100)
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.LastNameMaxLength));

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.PasswordRequired))
            .MinimumLength(8)
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.PasswordMinLength))
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)")
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.PasswordComplexity));

        RuleFor(x => x.StudyLevelTrackId)
            .NotEmpty()
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.StudyLevelTrackRequired));

        RuleFor(x => x.SchoolName)
            .MaximumLength(200)
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.SchoolNameMaxLength))
            .When(x => !string.IsNullOrWhiteSpace(x.SchoolName));

        RuleFor(x => x.Governorate)
            .MaximumLength(100)
            .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.GovernorateMaxLength))
            .When(x => !string.IsNullOrWhiteSpace(x.Governorate));
    }
}
```

#### 4. Application - Handler
```csharp
// Application/Auth/Commands/RegisterStudent/RegisterStudentCommandHandler.cs
public sealed class RegisterStudentCommandHandler
    : IRequestHandler<RegisterStudentCommand, ApiResult<UserInfoDto>>
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterStudentCommandHandler(
        IPasswordHasher passwordHasher,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _passwordHasher = passwordHasher;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult<UserInfoDto>> Handle(
        RegisterStudentCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Validate phone format
        var phoneResult = Phone.Create(request.Phone);
        if (phoneResult.IsFailure)
            return ApiResult<UserInfoDto>.Fail(
                phoneResult.Error.Code,
                ErrorMessages.GetMessage(phoneResult.Error.Code),  // Get Arabic message
                HttpStatusCodes.BadRequest);

        // 2. Check if phone already registered
        var existingUser = await _userRepository.GetByPhoneAsync(
            phoneResult.Value, cancellationToken);

        if (existingUser is not null)
            return ApiResult<UserInfoDto>.Fail(
                ErrorCodes.User.PhoneAlreadyExists,
                ErrorMessages.GetMessage(ErrorCodes.User.PhoneAlreadyExists),  // Arabic
                HttpStatusCodes.Conflict);

        // 3. Hash password
        var passwordHash = _passwordHasher.HashPassword(request.Password);

        // 4. Create user entity
        var userResult = User.Create(
            phoneResult.Value,
            passwordHash,
            request.FirstName,
            request.SecondName,
            request.LastName,
            UserType.Student,
            isPhoneVerified: true);

        if (userResult.IsFailure)
            return ApiResult<UserInfoDto>.Fail(
                userResult.Error.Code,
                ErrorMessages.GetMessage(userResult.Error.Code),  // Arabic
                HttpStatusCodes.BadRequest);

        var user = userResult.Value;

        // 5. Create student profile
        var profileResult = StudentProfile.Create(
            user.Id,
            request.StudyLevelTrackId,
            request.SchoolName,
            request.Governorate);

        if (profileResult.IsFailure)
            return ApiResult<UserInfoDto>.Fail(
                profileResult.Error.Code,
                ErrorMessages.GetMessage(profileResult.Error.Code),  // Arabic
                HttpStatusCodes.BadRequest);

        // 6. Save
        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.AddStudentProfileAsync(profileResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 7. Map to DTO and return with success message
        var dto = new UserInfoDto(
            user.Id.Value,
            user.GetFullName(),
            user.Phone.Value,
            user.UserType.ToString(),
            IsFirstLogin: false);

        return ApiResult<UserInfoDto>.Ok(
            dto,
            SuccessMessages.RegistrationSuccess,  // Arabic success message
            HttpStatusCodes.Created);
    }
}
```

#### 5. API - Controller
```csharp
// API/Controllers/AuthController.cs
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Register a new student account
    /// </summary>
    [HttpPost("register/student")]
    [AllowAnonymous]
    [ProducesResponseType<ApiResult<UserInfoDto>>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiResult>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiResult>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterStudent([FromBody] RegisterStudentRequest request)
    {
        var command = new RegisterStudentCommand(
            request.Phone,
            request.Password,
            request.FirstName,
            request.SecondName,
            request.LastName,
            request.StudyLevelTrackId,
            request.SchoolName,
            request.Governorate);

        var result = await _mediator.Send(command);
        return StatusCode(result.StatusCode, result);
    }
}

public record RegisterStudentRequest(
    string Phone,
    string Password,
    string FirstName,
    string SecondName,
    string LastName,
    Guid StudyLevelTrackId,
    string? SchoolName,
    string? Governorate);
```

---

### Query Example: GetAvailableCoursesQuery

**Simple read operation with filtering:**

#### 1. Application - Query
```csharp
// Application/Courses/Queries/GetAvailableCourses/GetAvailableCoursesQuery.cs
public sealed record GetAvailableCoursesQuery(
    Guid StudentId,
    Guid? SubjectId = null
) : IRequest<ApiResult<IEnumerable<CourseDto>>>;

// No validator needed - simple read operation
```

#### 2. Application - Handler
```csharp
// Application/Courses/Queries/GetAvailableCourses/GetAvailableCoursesQueryHandler.cs
public sealed class GetAvailableCoursesQueryHandler
    : IRequestHandler<GetAvailableCoursesQuery, ApiResult<IEnumerable<CourseDto>>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICourseRepository _courseRepository;

    public GetAvailableCoursesQueryHandler(
        IUserRepository userRepository,
        ICourseRepository courseRepository)
    {
        _userRepository = userRepository;
        _courseRepository = courseRepository;
    }

    public async Task<ApiResult<IEnumerable<CourseDto>>> Handle(
        GetAvailableCoursesQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Get student to determine their study level
        var student = await _userRepository.GetStudentByIdAsync(
            StudentId.From(request.StudentId),
            cancellationToken);

        if (student is null)
            return ApiResult<IEnumerable<CourseDto>>.Fail(
                ErrorCodes.User.NotFound,
                ErrorMessages.GetMessage(ErrorCodes.User.NotFound),
                HttpStatusCodes.NotFound);

        // 2. Get available courses for student's study level
        var subjectId = request.SubjectId.HasValue
            ? SubjectId.From(request.SubjectId.Value)
            : null;

        var courses = await _courseRepository.GetAvailableForStudentAsync(
            student.StudentProfile!.StudyLevelTrackId,
            subjectId,
            cancellationToken);

        // 3. Map to DTOs
        var dtos = courses.Select(c => new CourseDto(
            c.Id.Value,
            c.Title,
            c.Description,
            c.Price.Amount,
            c.TeacherName,
            c.SubjectName,
            c.TotalModules,
            c.TotalHours));

        // Success - simple queries can omit message or use generic one
        return ApiResult<IEnumerable<CourseDto>>.Ok(dtos);
    }
}
```

#### 3. API - Controller
```csharp
// API/Controllers/CoursesController.cs
[ApiController]
[Route("api/courses")]
[Authorize(Policy = "StudentOnly")]
public class CoursesController : ControllerBase
{
    private readonly IMediator _mediator;
    public CoursesController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Get courses available for current student
    /// </summary>
    [HttpGet]
    [ProducesResponseType<ApiResult<IEnumerable<CourseDto>>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAvailableCourses([FromQuery] Guid? subjectId)
    {
        var studentId = GetCurrentUserId();
        if (studentId == null)
            return Unauthorized(ApiResult.Fail(
                ErrorCodes.Auth.Unauthorized,
                ErrorMessages.GetMessage(ErrorCodes.Auth.Unauthorized),
                HttpStatusCodes.Unauthorized));

        var query = new GetAvailableCoursesQuery(studentId.Value, subjectId);
        var result = await _mediator.Send(query);
        return StatusCode(result.StatusCode, result);
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}
```

---

## Common Pitfalls

### ❌ DON'T Do This

```csharp
// 1. DON'T put messages in Domain layer
public static Result<Phone> Create(string phone)
{
    if (string.IsNullOrWhiteSpace(phone))
        return Result<Phone>.Failure(
            Error.Validation("USER.INVALID_PHONE", "Phone is required"));  // ❌ NO MESSAGE!
}

// 2. DON'T use hardcoded messages in handlers
if (phoneResult.IsFailure)
    return ApiResult<UserDto>.Fail(
        phoneResult.Error.Code,
        "Invalid phone number",  // ❌ Use ErrorMessages.GetMessage()
        HttpStatusCodes.BadRequest);

// Also DON'T use hardcoded success messages
return ApiResult<UserDto>.Ok(dto, "Registration successful");  // ❌ Use SuccessMessages

// 3. DON'T use hardcoded messages in validators
RuleFor(x => x.Phone)
    .NotEmpty()
    .WithMessage("Phone is required");  // ❌ Use ErrorMessages.GetMessage()

// 4. DON'T throw exceptions for business errors
if (assessment == null)
    throw new NotFoundException("Assessment not found");  // ❌ Use Result pattern

// 5. DON'T expose domain entities from API
[HttpGet("{id}")]
public async Task<User> GetUser(Guid id)  // ❌ Return DTOs
{
    return await _userRepository.GetByIdAsync(id);
}

// 6. DON'T put business logic in controllers
[HttpPost]
public async Task<IActionResult> CreateCourse(...)
{
    if (price < 0)  // ❌ Business rule in controller
        return BadRequest("Invalid price");
}

// 7. DON'T reference Infrastructure from Application
public class CreateCourseHandler
{
    private readonly ApplicationDbContext _context;  // ❌ Use IRepository
}

// 8. DON'T use public setters on domain entities
public class User
{
    public string FirstName { get; set; }  // ❌ Should be private set
}
```

### ✅ DO This Instead

```csharp
// 1. Return code-only errors from Domain
public static Result<Phone> Create(string phone)
{
    if (string.IsNullOrWhiteSpace(phone))
        return Result<Phone>.Failure(
            Error.Validation(ErrorCodes.User.InvalidPhone));  // ✅ Code only
}

// 2. Use ErrorMessages.GetMessage() and SuccessMessages in handlers
if (phoneResult.IsFailure)
    return ApiResult<UserDto>.Fail(
        phoneResult.Error.Code,
        ErrorMessages.GetMessage(phoneResult.Error.Code),  // ✅ Arabic error
        HttpStatusCodes.BadRequest);

// Return success with SuccessMessages
var dto = new UserDto(...);
return ApiResult<UserDto>.Ok(dto, SuccessMessages.RegistrationSuccess);  // ✅ Arabic success

// 3. Use ErrorMessages.GetMessage() in validators
RuleFor(x => x.Phone)
    .NotEmpty()
    .WithMessage(ErrorMessages.GetMessage(ErrorCodes.Validation.PhoneRequired));  // ✅

// 4. Use Result pattern
if (assessment is null)
    return ApiResult<AttemptDto>.Fail(
        ErrorCodes.Assessment.NotFound,
        ErrorMessages.GetMessage(ErrorCodes.Assessment.NotFound),
        HttpStatusCodes.NotFound);  // ✅

// 5. Return DTOs from API
[HttpGet("{id}")]
[ProducesResponseType<ApiResult<UserDto>>(StatusCodes.Status200OK)]
public async Task<IActionResult> GetUser(Guid id)  // ✅
{
    var query = new GetUserByIdQuery(id);
    var result = await _mediator.Send(query);
    return StatusCode(result.StatusCode, result);
}

// 6. Delegate to handlers
[HttpPost]
public async Task<IActionResult> CreateCourse(...)
{
    var command = new CreateCourseCommand(...);
    var result = await _mediator.Send(command);  // ✅
    return StatusCode(result.StatusCode, result);
}

// 7. Use repository interfaces
public class CreateCourseHandler
{
    private readonly ICourseRepository _repository;  // ✅
}

// 8. Use private setters with domain methods
public class User
{
    public string FirstName { get; private set; }  // ✅

    public Result UpdateName(string firstName, ...)
    {
        // Validation
        FirstName = firstName;  // ✅ Changed via method
        return Result.Success();
    }
}
```

---

## Quick Checklist

### Before Submitting a Feature

**Domain Layer:**
- [ ] Entities created with private constructors + factory methods
- [ ] Value objects used for complex types (Phone, Email, Money)
- [ ] Error **codes** defined in `ErrorCodes.cs`
- [ ] Domain methods return `Result` or `Result<T>`
- [ ] No messages in domain layer (codes only!)

**Application Layer:**
- [ ] Error **messages** added to `ErrorMessages.cs` (Arabic-only)
- [ ] Success **message** added to `SuccessMessages.cs` (Arabic)
- [ ] Commands/queries created
- [ ] Validators implemented using `ErrorMessages.GetMessage()`
- [ ] Handlers use `ErrorMessages.GetMessage()` for error responses
- [ ] Handlers use `SuccessMessages.XXX` for success responses
- [ ] Repository interfaces defined
- [ ] DTOs created for responses
- [ ] All async methods follow async/await pattern

**Infrastructure Layer:**
- [ ] Repository implementation in Infrastructure
- [ ] Entity configurations created
- [ ] Migration added if schema changed

**API Layer:**
- [ ] Controller endpoints with proper HTTP verbs
- [ ] Authorization attributes applied
- [ ] XML documentation comments added
- [ ] Request DTOs created
- [ ] `StatusCode(result.StatusCode, result)` returned

**Testing:**
- [ ] Build succeeds (0 errors)
- [ ] All tests pass
- [ ] API tested with Postman/curl
- [ ] Arabic success messages verified in responses
- [ ] Arabic error messages verified
- [ ] Validation errors verified (in Arabic)

**Code Quality:**
- [ ] No business logic in controllers
- [ ] No domain entities exposed from API
- [ ] No exceptions for business errors
- [ ] No hardcoded error messages (use `ErrorMessages.GetMessage()`)
- [ ] No hardcoded success messages (use `SuccessMessages`)
- [ ] All messages in Arabic only

---

## References

- **ERD Documentation**: `docs/LMS_ERD_Documentation_v2.md`
- **Implementation Plan**: `PLAN.md`
- **Result Pattern**: `src/LMS.Domain/Common/Result.cs`
- **Error Codes**: `src/LMS.Domain/Common/ErrorCodes.cs`
- **Error Messages**: `src/LMS.Application/Common/ErrorMessages.cs` (Arabic-only)
- **Success Messages**: `src/LMS.Application/Common/SuccessMessages.cs` (Arabic-only)
- **ApiResult Pattern**: `src/LMS.Application/Common/ApiResult.cs`

---

**Remember**:
- **Domain** = Code-only errors, business rules (no messages!)
- **Application** = Arabic messages (errors + success), orchestration
- **API** = Thin controllers, delegate to MediatR
- **Localization** = Arabic-only for Egyptian users
- **Success Messages** = Use `SuccessMessages` constants (DOZ pattern)
- **Clean Architecture** = Dependencies flow inward!
