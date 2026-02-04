# Egyptian LMS Implementation Patterns & Standards

**Purpose**: This document defines the architectural patterns, coding standards, and implementation workflow for the Egyptian LMS platform. Follow these patterns to ensure consistency, maintainability, and quality across all features.

**Last Updated**: February 4, 2026  
**Based On**: DOZ ride-sharing architecture + LMS-specific requirements

---

## Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [Layer Responsibilities](#layer-responsibilities)
3. [Project Structure](#project-structure)
4. [CQRS Pattern](#cqrs-pattern)
5. [Result Pattern](#result-pattern)
6. [Naming Conventions](#naming-conventions)
7. [Implementation Workflow](#implementation-workflow)
8. [Code Examples](#code-examples)
9. [Testing Standards](#testing-standards)
10. [Common Pitfalls](#common-pitfalls)

---

## Architecture Overview

```
┌─────────────────────────────────────────────────────────┐
│                     API Layer (ASP.NET)                 │
│  - Controllers (endpoints, auth, validation)            │
│  - Request/Response DTOs                                │
│  - ApiResult conversion                                 │
└────────────────────┬────────────────────────────────────┘
                     │ Uses MediatR
                     ↓
┌─────────────────────────────────────────────────────────┐
│                Application Layer (CQRS)                 │
│  - Commands & Queries                                   │
│  - Command/Query Handlers                               │
│  - FluentValidation Validators                          │
│  - Application Services (interfaces)                    │
│  - DTOs for data transfer                               │
└────────────────────┬────────────────────────────────────┘
                     │ Uses Repositories & Domain
                     ↓
┌─────────────────────────────────────────────────────────┐
│                   Domain Layer (DDD)                    │
│  - Aggregate Roots (entities)                           │
│  - Value Objects                                        │
│  - Domain Events                                        │
│  - Error Codes & Messages                               │
│  - Business Rules & Invariants                          │
└────────────────────┬────────────────────────────────────┘
                     │ Implemented by
                     ↓
┌─────────────────────────────────────────────────────────┐
│              Infrastructure Layer                        │
│  - Repository Implementations                           │
│  - DbContext & EF Core                                  │
│  - External Service Implementations                     │
│  - Configuration                                        │
└─────────────────────────────────────────────────────────┘
```

**Key Principles:**
- **Dependencies flow inward**: Infrastructure → Application → Domain
- **Domain is isolated**: No dependencies on other layers
- **Use interfaces**: Application defines interfaces, Infrastructure implements
- **Result Pattern**: No exceptions for business logic errors

---

## Layer Responsibilities

### 1. Domain Layer (`LMS.Domain`)

**What belongs here:**
- ✅ Aggregate Roots (e.g., `User`, `Course`, `Assessment`)
- ✅ Value Objects (e.g., `Phone`, `Email`, `Money`)
- ✅ Domain Events (e.g., `StudentEnrolledEvent`, `AssessmentGradedEvent`)
- ✅ Enums (e.g., `UserType`, `ContentType`, `AttemptStatus`)
- ✅ Error Codes & Messages (e.g., `ErrorCodes.Assessment.MaxRetakesExceeded`)
- ✅ Business rules & invariants
- ✅ Factory methods (`Create()` methods on entities)

**What does NOT belong here:**
- ❌ Database concerns (EF Core, migrations)
- ❌ External service calls (APIs, email, SMS)
- ❌ Application logic (orchestration)
- ❌ DTOs for API responses

**Key Patterns:**
```csharp
// Aggregate Root with factory method
public sealed class Course : AggregateRoot<CourseId>
{
    public CourseId Id { get; private set; }
    public string Title { get; private set; }
    public Money Price { get; private set; }
    public Visibility Visibility { get; private set; }

    // Private constructor - force factory usage
    private Course() { }

    // Factory method with validation
    public static Result<Course> Create(
        string title,
        Money price,
        TeacherId teacherId,
        StudyLevelTrackId studyLevelTrackId,
        SubjectId subjectId)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure<Course>(ErrorCodes.Course.TitleRequired);

        if (price.Amount < 0)
            return Result.Failure<Course>(ErrorCodes.Course.InvalidPrice);

        var course = new Course
        {
            Id = CourseId.CreateUnique(),
            Title = title,
            Price = price,
            TeacherId = teacherId,
            StudyLevelTrackId = studyLevelTrackId,
            SubjectId = subjectId,
            Visibility = Visibility.Draft,
            CreatedAtUtc = DateTime.UtcNow
        };

        course.RaiseDomainEvent(new CourseCreatedEvent(course.Id));

        return Result.Success(course);
    }

    // Domain methods
    public Result Publish()
    {
        if (Visibility == Visibility.Published)
            return Result.Failure(ErrorCodes.Course.AlreadyPublished);

        // Business rule: must have at least one module
        if (!Modules.Any())
            return Result.Failure(ErrorCodes.Course.NoModules);

        Visibility = Visibility.Published;
        UpdatedAtUtc = DateTime.UtcNow;

        RaiseDomainEvent(new CoursePublishedEvent(Id));

        return Result.Success();
    }
}

// Value Object
public sealed class Phone : ValueObject
{
    public string Value { get; private set; }

    private Phone(string value) => Value = value;

    public static Result<Phone> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<Phone>(ErrorCodes.User.PhoneRequired);

        // Egyptian phone format: 01XXXXXXXXX (11 digits starting with 01)
        var cleaned = value.Replace(" ", "").Replace("-", "");

        if (!Regex.IsMatch(cleaned, @"^01[0-9]{9}$"))
            return Result.Failure<Phone>(ErrorCodes.User.InvalidPhoneFormat);

        return Result.Success(new Phone(cleaned));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
```

---

### 2. Application Layer (`LMS.Application`)

**What belongs here:**
- ✅ Commands & Queries (MediatR requests)
- ✅ Command/Query Handlers
- ✅ FluentValidation Validators
- ✅ Application Service Interfaces (e.g., `IJwtTokenGenerator`, `IOtpService`)
- ✅ Repository Interfaces (e.g., `IUserRepository`, `ICourseRepository`)
- ✅ DTOs for API responses
- ✅ Mapping logic (Domain → DTO)

**Folder Structure:**
```
Application/
├── Common/
│   ├── ApiResult.cs                 # API response wrapper
│   ├── ResultExtensions.cs          # Result → ApiResult conversion
│   ├── HttpStatusCodes.cs           # Status code constants
│   ├── Interfaces/
│   │   ├── IRepository.cs
│   │   ├── IUnitOfWork.cs
│   │   ├── IJwtTokenGenerator.cs
│   │   ├── IOtpService.cs
│   │   └── IDateTimeProvider.cs
│   └── Behaviors/
│       ├── ValidationBehavior.cs    # MediatR pipeline
│       ├── LoggingBehavior.cs
│       └── TransactionBehavior.cs
├── {Feature}/                       # e.g., Auth, Content, Assessments
│   ├── Commands/
│   │   ├── {Action}/
│   │   │   ├── {Action}Command.cs
│   │   │   ├── {Action}CommandHandler.cs
│   │   │   └── {Action}CommandValidator.cs
│   ├── Queries/
│   │   ├── {Query}/
│   │   │   ├── {Query}Query.cs
│   │   │   └── {Query}QueryHandler.cs
│   ├── DTOs/
│   │   └── {Feature}Dto.cs
│   └── I{Feature}Repository.cs
```

**Key Patterns:**
```csharp
// Command
public sealed record CreateCourseCommand(
    Guid TeacherId,
    Guid StudyLevelTrackId,
    Guid SubjectId,
    string Title,
    string? Description,
    decimal Price
) : IRequest<ApiResult<CourseDto>>;

// Handler
public sealed class CreateCourseCommandHandler
    : IRequestHandler<CreateCourseCommand, ApiResult<CourseDto>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ITeacherRepository _teacherRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCourseCommandHandler(
        ICourseRepository courseRepository,
        ITeacherRepository teacherRepository,
        IUnitOfWork unitOfWork)
    {
        _courseRepository = courseRepository;
        _teacherRepository = teacherRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult<CourseDto>> Handle(
        CreateCourseCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Validate teacher exists
        var teacher = await _teacherRepository.GetByIdAsync(
            TeacherId.From(request.TeacherId), cancellationToken);

        if (teacher is null)
            return ApiResult<CourseDto>.Fail(
                ErrorCodes.Teacher.NotFound,
                "Teacher not found",
                HttpStatusCodes.NotFound);

        // 2. Create domain entity
        var priceResult = Money.Egp(request.Price);
        if (priceResult.IsFailure)
            return ApiResult<CourseDto>.Fail(
                priceResult.Error.Code,
                priceResult.Error.Message,
                HttpStatusCodes.BadRequest);

        var courseResult = Course.Create(
            request.Title,
            priceResult.Value,
            teacher.Id,
            StudyLevelTrackId.From(request.StudyLevelTrackId),
            SubjectId.From(request.SubjectId));

        if (courseResult.IsFailure)
            return ApiResult<CourseDto>.Fail(
                courseResult.Error.Code,
                courseResult.Error.Message,
                HttpStatusCodes.BadRequest);

        // 3. Persist
        await _courseRepository.AddAsync(courseResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 4. Map to DTO
        var dto = MapToDto(courseResult.Value);

        return ApiResult<CourseDto>.Ok(dto, HttpStatusCodes.Created);
    }

    private static CourseDto MapToDto(Course course) => new(
        course.Id.Value,
        course.Title,
        course.Description,
        course.Price.Amount,
        course.Visibility.ToString(),
        course.CreatedAtUtc
    );
}

// Validator
public sealed class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(300)
            .WithMessage("Title must be less than 300 characters");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Price must be greater than or equal to 0");

        RuleFor(x => x.TeacherId)
            .NotEmpty()
            .WithMessage("Teacher ID is required");

        RuleFor(x => x.StudyLevelTrackId)
            .NotEmpty()
            .WithMessage("Study level track ID is required");

        RuleFor(x => x.SubjectId)
            .NotEmpty()
            .WithMessage("Subject ID is required");
    }
}
```

---

### 3. Infrastructure Layer (`LMS.Infrastructure`)

**What belongs here:**
- ✅ Repository implementations
- ✅ DbContext and entity configurations
- ✅ External service implementations (VdoCipher, Paymob, OneSignal)
- ✅ Caching implementations (Redis)
- ✅ Background job implementations (Hangfire)
- ✅ Authentication implementations (JWT)

**Key Patterns:**
```csharp
// Repository Implementation
public sealed class CourseRepository : Repository<Course, CourseId>, ICourseRepository
{
    public CourseRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Course?> GetByIdWithModulesAsync(
        CourseId id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Courses
            .Include(c => c.Modules.OrderBy(m => m.DisplayOrder))
            .ThenInclude(m => m.Stages.OrderBy(s => s.DisplayOrder))
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Course>> GetByTeacherIdAsync(
        TeacherId teacherId,
        bool includeUnpublished = false,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Courses
            .Where(c => c.TeacherId == teacherId);

        if (!includeUnpublished)
            query = query.Where(c => c.Visibility == Visibility.Published);

        return await query
            .OrderByDescending(c => c.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Course>> GetAvailableForStudentAsync(
        StudyLevelTrackId studyLevelTrackId,
        SubjectId? subjectId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Courses
            .Where(c => c.StudyLevelTrackId == studyLevelTrackId)
            .Where(c => c.Visibility == Visibility.Published)
            .Where(c => c.IsActive);

        if (subjectId.HasValue)
            query = query.Where(c => c.SubjectId == subjectId.Value);

        return await query
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync(cancellationToken);
    }
}

// Entity Configuration
public sealed class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Course");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasConversion(
                id => id.Value,
                value => CourseId.From(value))
            .HasDefaultValueSql("uuid_generate_v7()");

        builder.Property(c => c.Title)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasColumnType("text");

        builder.OwnsOne(c => c.Price, priceBuilder =>
        {
            priceBuilder.Property(m => m.Amount)
                .HasColumnName("FullPrice")
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            priceBuilder.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .HasDefaultValue("EGP");
        });

        builder.Property(c => c.Visibility)
            .HasConversion<int>()
            .IsRequired();

        builder.HasIndex(c => new { c.StudyLevelTrackId, c.SubjectId })
            .HasFilter("\"Visibility\" = 2 AND \"IsActive\" = true")
            .HasDatabaseName("IX_Course_Published");

        // Relationships
        builder.HasOne(c => c.Teacher)
            .WithMany(t => t.Courses)
            .HasForeignKey(c => c.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Modules)
            .WithOne(m => m.Course)
            .HasForeignKey(m => m.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

---

### 4. API Layer (`LMS.API`)

**What belongs here:**
- ✅ Controllers (HTTP endpoints)
- ✅ Request DTOs (simplified input models)
- ✅ Authorization attributes
- ✅ Swagger documentation
- ✅ Middleware (error handling, logging)

**Key Patterns:**
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CoursesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CoursesController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Create a new course
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "TeacherOnly")]
    [ProducesResponseType<ApiResult<CourseDto>>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiResult>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateCourse([FromBody] CreateCourseRequest request)
    {
        var teacherId = GetCurrentUserId();
        if (teacherId == null)
            return Unauthorized(ApiResult.Fail("User ID not found", HttpStatusCodes.Unauthorized));

        var command = new CreateCourseCommand(
            teacherId.Value,
            request.StudyLevelTrackId,
            request.SubjectId,
            request.Title,
            request.Description,
            request.Price);

        var result = await _mediator.Send(command);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Publish a course
    /// </summary>
    [HttpPost("{id:guid}/publish")]
    [Authorize(Policy = "TeacherOnly")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResult>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PublishCourse(Guid id)
    {
        var teacherId = GetCurrentUserId();
        if (teacherId == null)
            return Unauthorized(ApiResult.Fail("User ID not found", HttpStatusCodes.Unauthorized));

        var command = new PublishCourseCommand(id, teacherId.Value);
        var result = await _mediator.Send(command);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Get courses available for current student
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "StudentOnly")]
    [ProducesResponseType<ApiResult<IEnumerable<CourseDto>>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailableCourses([FromQuery] Guid? subjectId)
    {
        var studentId = GetCurrentUserId();
        if (studentId == null)
            return Unauthorized(ApiResult.Fail("User ID not found", HttpStatusCodes.Unauthorized));

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

// Request DTO
public record CreateCourseRequest(
    Guid StudyLevelTrackId,
    Guid SubjectId,
    string Title,
    string? Description,
    decimal Price);
```

---

## CQRS Pattern

### Command vs Query

| Aspect | Command | Query |
|--------|---------|-------|
| Purpose | Modify state | Read data |
| Return | Success/Failure | Data |
| Side Effects | Yes (domain events) | No |
| Caching | No | Yes (where appropriate) |

### Example Flow

```
User Request → Controller → Command/Query → Handler → Domain → Repository → Database
                                                  ↓
                                            Domain Event
                                                  ↓
                                         Event Handler (async)
                                                  ↓
                                         Send Notification / Award XP
```

---

## Result Pattern

### Why Result Pattern?

1. **Explicit failure handling**: No hidden exceptions
2. **Type-safe errors**: Error codes are strings, not exception types
3. **Testable**: Easy to assert on success/failure
4. **Composable**: Can chain operations with `Then()`, `Map()`

### Implementation

```csharp
// Result without value
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    protected Result(bool isSuccess, Error error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);
    public static Result Failure(string code, string message) =>
        new(false, new Error(code, message));
}

// Result with value
public class Result<T> : Result
{
    public T Value { get; }

    private Result(T value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(value, true, Error.None);
    public static new Result<T> Failure(Error error) => new(default!, false, error);
    public static new Result<T> Failure(string code, string message) =>
        new(default!, false, new Error(code, message));
}

// Error
public record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);
}

// API Result (for HTTP responses)
public class ApiResult
{
    public bool Success { get; init; }
    public int StatusCode { get; init; }
    public object? Data { get; init; }
    public ErrorDetails? Error { get; init; }

    public static ApiResult Ok(int statusCode = HttpStatusCodes.Ok) =>
        new() { Success = true, StatusCode = statusCode };

    public static ApiResult Fail(string code, string message, int statusCode) =>
        new()
        {
            Success = false,
            StatusCode = statusCode,
            Error = new ErrorDetails { Code = code, Message = message }
        };
}

public class ApiResult<T> : ApiResult
{
    public new T? Data { get; init; }

    public static ApiResult<T> Ok(T data, int statusCode = HttpStatusCodes.Ok) =>
        new() { Success = true, Data = data, StatusCode = statusCode };

    public static new ApiResult<T> Fail(string code, string message, int statusCode) =>
        new()
        {
            Success = false,
            StatusCode = statusCode,
            Error = new ErrorDetails { Code = code, Message = message }
        };
}
```

---

## Implementation Workflow

### For Each Feature

1. **Domain Layer**
   - Create/update aggregate root
   - Create value objects if needed
   - Define domain events
   - Add error codes to `ErrorCodes.cs`

2. **Application Layer**
   - Create command/query record
   - Create validator
   - Create handler
   - Create DTOs
   - Add repository interface if needed

3. **Infrastructure Layer**
   - Add entity configuration
   - Implement repository methods
   - Add migration if schema changed

4. **API Layer**
   - Create controller endpoint
   - Add request DTOs
   - Add authorization
   - Document with XML comments

5. **Testing**
   - Unit test domain logic
   - Unit test handlers
   - Integration test API endpoints

---

## Code Examples

### Complete Feature: Start Assessment Attempt

#### 1. Domain

```csharp
// Domain/Assessments/StudentAttempt.cs
public sealed class StudentAttempt : Entity<StudentAttemptId>
{
    public StudentAttemptId Id { get; private set; }
    public StudentId StudentId { get; private set; }
    public AssessmentId AssessmentId { get; private set; }
    public int AttemptNumber { get; private set; }
    public AttemptStatus Status { get; private set; }
    public DateTime StartedAtUtc { get; private set; }
    public DateTime? SubmittedAtUtc { get; private set; }
    public DateTime? DueAtUtc { get; private set; }
    public decimal? TotalScore { get; private set; }
    public decimal? PercentageScore { get; private set; }
    public bool? IsPassed { get; private set; }

    private readonly List<StudentAnswer> _answers = new();
    public IReadOnlyCollection<StudentAnswer> Answers => _answers.AsReadOnly();

    private StudentAttempt() { }

    public static Result<StudentAttempt> Create(
        StudentId studentId,
        Assessment assessment,
        int attemptNumber,
        DateTime now)
    {
        // Business rules
        if (attemptNumber > assessment.MaxRetakes)
            return Result.Failure<StudentAttempt>(
                ErrorCodes.Assessment.MaxRetakesExceeded);

        var attempt = new StudentAttempt
        {
            Id = StudentAttemptId.CreateUnique(),
            StudentId = studentId,
            AssessmentId = assessment.Id,
            AttemptNumber = attemptNumber,
            Status = AttemptStatus.InProgress,
            StartedAtUtc = now,
            DueAtUtc = assessment.DurationMinutes.HasValue
                ? now.AddMinutes(assessment.DurationMinutes.Value)
                : null
        };

        return Result.Success(attempt);
    }

    public Result Submit(DateTime now)
    {
        if (Status != AttemptStatus.InProgress)
            return Result.Failure(ErrorCodes.Assessment.AlreadySubmitted);

        Status = AttemptStatus.Submitted;
        SubmittedAtUtc = now;

        return Result.Success();
    }

    public Result Grade(decimal totalScore, decimal passingMarks)
    {
        if (Status != AttemptStatus.Submitted)
            return Result.Failure(ErrorCodes.Assessment.NotSubmitted);

        TotalScore = totalScore;
        PercentageScore = (totalScore / passingMarks) * 100;
        IsPassed = totalScore >= passingMarks;
        Status = AttemptStatus.Graded;

        return Result.Success();
    }
}
```

#### 2. Application - Command

```csharp
// Application/Assessments/Commands/StartAttempt/StartAttemptCommand.cs
public sealed record StartAttemptCommand(
    Guid StudentId,
    Guid AssessmentId
) : IRequest<ApiResult<AttemptDto>>;
```

#### 3. Application - Validator

```csharp
// Application/Assessments/Commands/StartAttempt/StartAttemptCommandValidator.cs
public sealed class StartAttemptCommandValidator : AbstractValidator<StartAttemptCommand>
{
    public StartAttemptCommandValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty()
            .WithMessage("Student ID is required");

        RuleFor(x => x.AssessmentId)
            .NotEmpty()
            .WithMessage("Assessment ID is required");
    }
}
```

#### 4. Application - Handler

```csharp
// Application/Assessments/Commands/StartAttempt/StartAttemptCommandHandler.cs
public sealed class StartAttemptCommandHandler
    : IRequestHandler<StartAttemptCommand, ApiResult<AttemptDto>>
{
    private readonly IAssessmentRepository _assessmentRepository;
    private readonly IStudentAttemptRepository _attemptRepository;
    private readonly IStudentAssessmentScoreRepository _scoreRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public StartAttemptCommandHandler(
        IAssessmentRepository assessmentRepository,
        IStudentAttemptRepository attemptRepository,
        IStudentAssessmentScoreRepository scoreRepository,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _assessmentRepository = assessmentRepository;
        _attemptRepository = attemptRepository;
        _scoreRepository = scoreRepository;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult<AttemptDto>> Handle(
        StartAttemptCommand request,
        CancellationToken cancellationToken)
    {
        var studentId = StudentId.From(request.StudentId);
        var assessmentId = AssessmentId.From(request.AssessmentId);

        // 1. Get assessment
        var assessment = await _assessmentRepository.GetByIdAsync(
            assessmentId, cancellationToken);

        if (assessment is null)
            return ApiResult<AttemptDto>.Fail(
                ErrorCodes.Assessment.NotFound,
                "Assessment not found",
                HttpStatusCodes.NotFound);

        // 2. Check if student has an in-progress attempt
        var existingAttempt = await _attemptRepository.GetInProgressAsync(
            studentId, assessmentId, cancellationToken);

        if (existingAttempt is not null)
            return ApiResult<AttemptDto>.Fail(
                ErrorCodes.Assessment.AttemptInProgress,
                "You already have an in-progress attempt",
                HttpStatusCodes.Conflict);

        // 3. Check cooldown
        var score = await _scoreRepository.GetAsync(
            studentId, assessmentId, cancellationToken);

        if (score is not null)
        {
            var now = _dateTimeProvider.UtcNow;
            if (score.NextRetryAvailableAtUtc.HasValue &&
                score.NextRetryAvailableAtUtc.Value > now)
            {
                return ApiResult<AttemptDto>.Fail(
                    ErrorCodes.Assessment.CooldownActive,
                    $"Please wait until {score.NextRetryAvailableAtUtc.Value:HH:mm} to retry",
                    HttpStatusCodes.TooManyRequests);
            }
        }

        // 4. Determine attempt number
        var attemptNumber = (score?.TotalAttempts ?? 0) + 1;

        // 5. Create attempt
        var attemptResult = StudentAttempt.Create(
            studentId,
            assessment,
            attemptNumber,
            _dateTimeProvider.UtcNow);

        if (attemptResult.IsFailure)
            return ApiResult<AttemptDto>.Fail(
                attemptResult.Error.Code,
                attemptResult.Error.Message,
                HttpStatusCodes.BadRequest);

        // 6. Save
        await _attemptRepository.AddAsync(attemptResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 7. Map to DTO
        var dto = MapToDto(attemptResult.Value, assessment);

        return ApiResult<AttemptDto>.Ok(dto, HttpStatusCodes.Created);
    }

    private static AttemptDto MapToDto(StudentAttempt attempt, Assessment assessment) => new(
        attempt.Id.Value,
        attempt.AssessmentId.Value,
        assessment.Title,
        attempt.AttemptNumber,
        attempt.Status.ToString(),
        attempt.StartedAtUtc,
        attempt.DueAtUtc,
        assessment.DurationMinutes,
        assessment.Questions.Select(q => new QuestionDto(
            q.QuestionId.Value,
            q.Question.Text,
            q.Question.QuestionType.ToString(),
            q.Marks,
            q.Question.Options.Select(o => new OptionDto(o.Id.Value, o.Text)).ToList()
        )).ToList()
    );
}
```

#### 5. Infrastructure - Repository

```csharp
// Infrastructure/Persistence/Repositories/StudentAttemptRepository.cs
public sealed class StudentAttemptRepository : Repository<StudentAttempt, StudentAttemptId>,
    IStudentAttemptRepository
{
    public StudentAttemptRepository(ApplicationDbContext context) : base(context) { }

    public async Task<StudentAttempt?> GetInProgressAsync(
        StudentId studentId,
        AssessmentId assessmentId,
        CancellationToken cancellationToken = default)
    {
        return await _context.StudentAttempts
            .FirstOrDefaultAsync(a =>
                a.StudentId == studentId &&
                a.AssessmentId == assessmentId &&
                a.Status == AttemptStatus.InProgress,
                cancellationToken);
    }
}
```

#### 6. API Controller

```csharp
// API/Controllers/AttemptsController.cs
[ApiController]
[Route("api/assessments/{assessmentId:guid}/attempts")]
[Authorize(Policy = "StudentOnly")]
public class AttemptsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AttemptsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Start a new attempt for an assessment
    /// </summary>
    [HttpPost]
    [ProducesResponseType<ApiResult<AttemptDto>>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiResult>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiResult>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiResult>(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> StartAttempt(Guid assessmentId)
    {
        var studentId = GetCurrentUserId();
        if (studentId == null)
            return Unauthorized(ApiResult.Fail(
                "AUTH.UNAUTHORIZED",
                "User ID not found",
                HttpStatusCodes.Unauthorized));

        var command = new StartAttemptCommand(studentId.Value, assessmentId);
        var result = await _mediator.Send(command);
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

## Testing Standards

### Unit Tests Structure

```csharp
public class StartAttemptCommandHandlerTests
{
    private readonly Mock<IAssessmentRepository> _assessmentRepositoryMock;
    private readonly Mock<IStudentAttemptRepository> _attemptRepositoryMock;
    private readonly Mock<IStudentAssessmentScoreRepository> _scoreRepositoryMock;
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly StartAttemptCommandHandler _handler;

    public StartAttemptCommandHandlerTests()
    {
        _assessmentRepositoryMock = new Mock<IAssessmentRepository>();
        _attemptRepositoryMock = new Mock<IStudentAttemptRepository>();
        _scoreRepositoryMock = new Mock<IStudentAssessmentScoreRepository>();
        _dateTimeProviderMock = new Mock<IDateTimeProvider>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new StartAttemptCommandHandler(
            _assessmentRepositoryMock.Object,
            _attemptRepositoryMock.Object,
            _scoreRepositoryMock.Object,
            _dateTimeProviderMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateAttempt()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var assessmentId = Guid.NewGuid();
        var command = new StartAttemptCommand(studentId, assessmentId);

        var assessment = CreateTestAssessment(assessmentId);

        _assessmentRepositoryMock
            .Setup(x => x.GetByIdAsync(
                It.IsAny<AssessmentId>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(assessment);

        _attemptRepositoryMock
            .Setup(x => x.GetInProgressAsync(
                It.IsAny<StudentId>(),
                It.IsAny<AssessmentId>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((StudentAttempt?)null);

        _scoreRepositoryMock
            .Setup(x => x.GetAsync(
                It.IsAny<StudentId>(),
                It.IsAny<AssessmentId>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((StudentAssessmentScore?)null);

        _dateTimeProviderMock
            .Setup(x => x.UtcNow)
            .Returns(DateTime.UtcNow);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCodes.Created);
        result.Data.Should().NotBeNull();
        result.Data!.AttemptNumber.Should().Be(1);

        _attemptRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<StudentAttempt>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_AssessmentNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var command = new StartAttemptCommand(Guid.NewGuid(), Guid.NewGuid());

        _assessmentRepositoryMock
            .Setup(x => x.GetByIdAsync(
                It.IsAny<AssessmentId>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Assessment?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCodes.NotFound);
        result.Error!.Code.Should().Be(ErrorCodes.Assessment.NotFound);
    }

    [Fact]
    public async Task Handle_AttemptInProgress_ShouldReturnConflict()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var assessmentId = Guid.NewGuid();
        var command = new StartAttemptCommand(studentId, assessmentId);

        var assessment = CreateTestAssessment(assessmentId);
        var existingAttempt = CreateTestAttempt(studentId, assessmentId);

        _assessmentRepositoryMock
            .Setup(x => x.GetByIdAsync(
                It.IsAny<AssessmentId>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(assessment);

        _attemptRepositoryMock
            .Setup(x => x.GetInProgressAsync(
                It.IsAny<StudentId>(),
                It.IsAny<AssessmentId>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingAttempt);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCodes.Conflict);
        result.Error!.Code.Should().Be(ErrorCodes.Assessment.AttemptInProgress);
    }

    private static Assessment CreateTestAssessment(Guid id) => // ... create test assessment
    private static StudentAttempt CreateTestAttempt(Guid studentId, Guid assessmentId) => // ... create test attempt
}
```

---

## Common Pitfalls

### ❌ Don't Do This

```csharp
// 1. Don't throw exceptions for business errors
public async Task<AttemptDto> StartAttempt(...)
{
    if (assessment == null)
        throw new NotFoundException("Assessment not found"); // ❌
}

// 2. Don't expose domain entities from API
[HttpGet("{id}")]
public async Task<Course> GetCourse(Guid id) // ❌
{
    return await _courseRepository.GetByIdAsync(id); // ❌
}

// 3. Don't put business logic in controllers
[HttpPost]
public async Task<IActionResult> StartAttempt(...)
{
    if (attemptNumber > maxRetakes) // ❌ Business rule in controller
        return BadRequest("Too many retakes");
}

// 4. Don't reference Infrastructure from Application
public class StartAttemptHandler
{
    private readonly ApplicationDbContext _context; // ❌
}

// 5. Don't use DTOs in domain layer
public class Assessment
{
    public AssessmentDto ToDto() { ... } // ❌
}

// 6. Don't skip validation
public class StartAttemptCommand : IRequest<ApiResult<AttemptDto>>
{
    // No validator created ❌
}

// 7. Don't use public setters on domain entities
public class Assessment
{
    public AttemptStatus Status { get; set; } // ❌ Should be private set
}
```

### ✅ Do This Instead

```csharp
// 1. Use Result pattern
public async Task<ApiResult<AttemptDto>> Handle(...)
{
    if (assessment == null)
        return ApiResult<AttemptDto>.Fail(
            ErrorCodes.Assessment.NotFound,
            "Assessment not found",
            HttpStatusCodes.NotFound); // ✅
}

// 2. Return DTOs from API
[HttpGet("{id}")]
[ProducesResponseType<ApiResult<CourseDto>>(StatusCodes.Status200OK)]
public async Task<IActionResult> GetCourse(Guid id) // ✅

// 3. Keep controllers thin - delegate to handlers
[HttpPost]
public async Task<IActionResult> StartAttempt(...)
{
    var command = new StartAttemptCommand(...);
    var result = await _mediator.Send(command); // ✅
    return StatusCode(result.StatusCode, result);
}

// 4. Use repository interfaces in Application
public class StartAttemptHandler
{
    private readonly IAssessmentRepository _repository; // ✅
}

// 5. Map in handlers
public async Task<ApiResult<AssessmentDto>> Handle(...)
{
    var assessment = ...;
    var dto = new AssessmentDto { ... }; // ✅
    return ApiResult<AssessmentDto>.Ok(dto);
}

// 6. Always create validators
public class StartAttemptCommandValidator : AbstractValidator<StartAttemptCommand>
{
    public StartAttemptCommandValidator()
    {
        RuleFor(x => x.AssessmentId).NotEmpty(); // ✅
    }
}

// 7. Use private setters with domain methods
public class Assessment
{
    public AttemptStatus Status { get; private set; } // ✅

    public Result Submit()
    {
        Status = AttemptStatus.Submitted; // ✅ Changed via method
        return Result.Success();
    }
}
```

---

## Quick Checklist

Before submitting a feature:

- [ ] Domain entities created with factory methods
- [ ] Value objects used for complex types
- [ ] Error codes and messages defined
- [ ] Commands and queries created
- [ ] Validators implemented for all commands
- [ ] Handlers follow async/await pattern
- [ ] Repository interface defined in Application
- [ ] Repository implementation in Infrastructure
- [ ] Controller endpoints with proper HTTP verbs
- [ ] Authorization attributes applied
- [ ] XML documentation comments added
- [ ] ApiResult<T> used for all responses
- [ ] Unit tests for handlers
- [ ] Integration tests for API
- [ ] PLAN.md updated with progress
- [ ] No business logic in controllers
- [ ] No domain entities exposed from API
- [ ] No exceptions for business errors

---

## References

- **ERD Documentation**: `docs/LMS_ERD_Documentation_v2.md`
- **Implementation Plan**: `PLAN.md`
- **Domain Layer**: `src/LMS.Domain/`
- **Result Pattern**: `src/LMS.Domain/Common/Result.cs`
- **ApiResult Pattern**: `src/LMS.Application/Common/ApiResult.cs`

---

**Remember**: Consistency is key. When in doubt, follow the patterns in this document!
