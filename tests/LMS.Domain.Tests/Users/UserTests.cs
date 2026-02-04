using FluentAssertions;
using LMS.Domain.Common;
using LMS.Domain.Users;

namespace LMS.Domain.Tests.Users;

public sealed class UserTests
{
    private readonly Phone _validPhone;
    private const string ValidPasswordHash = "hashed_password_here";
    private const string ValidFullName = "Ahmed Mohamed";

    public UserTests()
    {
        _validPhone = Phone.Create("01012345678").Value;
    }

    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Act
        var result = User.Create(
            _validPhone,
            ValidPasswordHash,
            ValidFullName,
            UserType.Student,
            isPhoneVerified: false);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var user = result.Value;
        user.Phone.Should().Be(_validPhone);
        user.PasswordHash.Should().Be(ValidPasswordHash);
        user.FullName.Should().Be(ValidFullName);
        user.UserType.Should().Be(UserType.Student);
        user.IsPhoneVerified.Should().BeFalse();
        user.IsActive.Should().BeTrue();
        user.CreatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_WithEmptyPasswordHash_ShouldFail()
    {
        // Act
        var result = User.Create(
            _validPhone,
            string.Empty,
            ValidFullName,
            UserType.Student);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("VALIDATION.REQUIRED");
        result.Error.Message.Should().Contain("Password hash is required");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidFullName_ShouldFail(string fullName)
    {
        // Act
        var result = User.Create(
            _validPhone,
            ValidPasswordHash,
            fullName,
            UserType.Student);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("VALIDATION.REQUIRED");
        result.Error.Message.Should().Contain("Full name is required");
    }

    [Fact]
    public void Create_WithTooLongFullName_ShouldFail()
    {
        // Arrange
        var longName = new string('A', 201);

        // Act
        var result = User.Create(
            _validPhone,
            ValidPasswordHash,
            longName,
            UserType.Student);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("VALIDATION.INVALID_INPUT");
        result.Error.Message.Should().Contain("Full name cannot exceed 200 characters");
    }

    [Fact]
    public void CreateParent_WithValidPhone_ShouldSucceed()
    {
        // Act
        var result = User.CreateParent(_validPhone, ValidPasswordHash);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var parent = result.Value;
        parent.Phone.Should().Be(_validPhone);
        parent.PasswordHash.Should().Be(ValidPasswordHash);
        parent.FullName.Should().Be("Parent");
        parent.UserType.Should().Be(UserType.Parent);
        parent.IsPhoneVerified.Should().BeTrue();
        parent.IsActive.Should().BeTrue();
    }

    [Fact]
    public void VerifyPhone_WhenNotVerified_ShouldSucceed()
    {
        // Arrange
        var user = User.Create(_validPhone, ValidPasswordHash, ValidFullName, UserType.Student).Value;

        // Act
        var result = user.VerifyPhone();

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.IsPhoneVerified.Should().BeTrue();
        user.UpdatedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void VerifyPhone_WhenAlreadyVerified_ShouldFail()
    {
        // Arrange
        var user = User.Create(_validPhone, ValidPasswordHash, ValidFullName, UserType.Student, isPhoneVerified: true).Value;

        // Act
        var result = user.VerifyPhone();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("USER.ALREADY_VERIFIED");
    }

    [Fact]
    public void UpdatePassword_WithValidPasswordHash_ShouldSucceed()
    {
        // Arrange
        var user = User.Create(_validPhone, ValidPasswordHash, ValidFullName, UserType.Student).Value;
        const string newPasswordHash = "new_hashed_password";

        // Act
        var result = user.UpdatePassword(newPasswordHash);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.PasswordHash.Should().Be(newPasswordHash);
        user.UpdatedAtUtc.Should().NotBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void UpdatePassword_WithInvalidPasswordHash_ShouldFail(string newPasswordHash)
    {
        // Arrange
        var user = User.Create(_validPhone, ValidPasswordHash, ValidFullName, UserType.Student).Value;

        // Act
        var result = user.UpdatePassword(newPasswordHash);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("VALIDATION.REQUIRED");
    }

    [Fact]
    public void UpdateProfile_WithValidData_ShouldSucceed()
    {
        // Arrange
        var user = User.Create(_validPhone, ValidPasswordHash, ValidFullName, UserType.Student).Value;
        const string newFullName = "Mohamed Ahmed";
        const string email = "test@example.com";

        // Act
        var result = user.UpdateProfile(newFullName, email);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.FullName.Should().Be(newFullName);
        user.Email.Should().Be(email);
        user.UpdatedAtUtc.Should().NotBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void UpdateProfile_WithInvalidFullName_ShouldFail(string newFullName)
    {
        // Arrange
        var user = User.Create(_validPhone, ValidPasswordHash, ValidFullName, UserType.Student).Value;

        // Act
        var result = user.UpdateProfile(newFullName);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("VALIDATION.REQUIRED");
    }

    [Fact]
    public void UpdateProfile_WithTooLongFullName_ShouldFail()
    {
        // Arrange
        var user = User.Create(_validPhone, ValidPasswordHash, ValidFullName, UserType.Student).Value;
        var longName = new string('A', 201);

        // Act
        var result = user.UpdateProfile(longName);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("VALIDATION.INVALID_INPUT");
    }

    [Fact]
    public void Deactivate_WhenActive_ShouldSucceed()
    {
        // Arrange
        var user = User.Create(_validPhone, ValidPasswordHash, ValidFullName, UserType.Student).Value;
        const string reason = "Account suspended";

        // Act
        var result = user.Deactivate(reason);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.IsActive.Should().BeFalse();
        user.UpdatedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void Deactivate_WhenAlreadyInactive_ShouldFail()
    {
        // Arrange
        var user = User.Create(_validPhone, ValidPasswordHash, ValidFullName, UserType.Student).Value;
        user.Deactivate("First deactivation");

        // Act
        var result = user.Deactivate("Second deactivation");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("USER.INACTIVE");
    }

    [Fact]
    public void Reactivate_WhenInactive_ShouldSucceed()
    {
        // Arrange
        var user = User.Create(_validPhone, ValidPasswordHash, ValidFullName, UserType.Student).Value;
        user.Deactivate("Test");

        // Act
        var result = user.Reactivate();

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.IsActive.Should().BeTrue();
        user.UpdatedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void Reactivate_WhenAlreadyActive_ShouldFail()
    {
        // Arrange
        var user = User.Create(_validPhone, ValidPasswordHash, ValidFullName, UserType.Student).Value;

        // Act
        var result = user.Reactivate();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("VALIDATION.INVALID_INPUT");
    }

    [Fact]
    public void CanLogin_WhenActiveAndVerified_ShouldSucceed()
    {
        // Arrange
        var user = User.Create(_validPhone, ValidPasswordHash, ValidFullName, UserType.Student, isPhoneVerified: true).Value;

        // Act
        var result = user.CanLogin();

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void CanLogin_WhenInactive_ShouldFail()
    {
        // Arrange
        var user = User.Create(_validPhone, ValidPasswordHash, ValidFullName, UserType.Student, isPhoneVerified: true).Value;
        user.Deactivate("Test");

        // Act
        var result = user.CanLogin();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("USER.INACTIVE");
    }

    [Fact]
    public void CanLogin_WhenNotVerified_ShouldFail()
    {
        // Arrange
        var user = User.Create(_validPhone, ValidPasswordHash, ValidFullName, UserType.Student, isPhoneVerified: false).Value;

        // Act
        var result = user.CanLogin();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("USER.NOT_VERIFIED");
    }
}
