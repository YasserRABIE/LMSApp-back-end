using FluentAssertions;
using LMS.Domain.Common;
using LMS.Domain.Users;

namespace LMS.Domain.Tests.Users;

public sealed class UserTests
{
    private readonly Phone _validPhone;
    private const string ValidPasswordHash = "hashed_password_here";
    private const string ValidFirstName = "Ahmed";
    private const string ValidSecondName = "Mohamed";
    private const string ValidLastName = "Hassan";

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
            ValidFirstName,
            ValidSecondName,
            ValidLastName,
            UserType.Student,
            isPhoneVerified: false);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var user = result.Value;
        user.Phone.Should().Be(_validPhone);
        user.PasswordHash.Should().Be(ValidPasswordHash);
        user.FirstName.Should().Be(ValidFirstName);
        user.SecondName.Should().Be(ValidSecondName);
        user.LastName.Should().Be(ValidLastName);
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
            ValidFirstName,
            ValidSecondName,
            ValidLastName,
            UserType.Student);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(ErrorCodes.Validation.Required);
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidFirstName_ShouldFail(string firstName)
    {
        // Act
        var result = User.Create(
            _validPhone,
            ValidPasswordHash,
            firstName,
            ValidSecondName,
            ValidLastName,
            UserType.Student);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(ErrorCodes.Validation.Required);
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public void Create_WithTooLongFirstName_ShouldFail()
    {
        // Arrange
        var longName = new string('A', 101);

        // Act
        var result = User.Create(
            _validPhone,
            ValidPasswordHash,
            longName,
            ValidSecondName,
            ValidLastName,
            UserType.Student);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(ErrorCodes.Validation.InvalidInput);
        result.Error.Type.Should().Be(ErrorType.Validation);
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
        parent.FirstName.Should().Be("Parent");
        parent.SecondName.Should().Be("");
        parent.LastName.Should().Be("");
        parent.UserType.Should().Be(UserType.Parent);
        parent.IsPhoneVerified.Should().BeTrue();
        parent.IsActive.Should().BeTrue();
    }

    [Fact]
    public void VerifyPhone_WhenNotVerified_ShouldSucceed()
    {
        // Arrange
        var user = User.Create(_validPhone, ValidPasswordHash, ValidFirstName, ValidSecondName, ValidLastName, UserType.Student).Value;

        // Act
        var result = user.VerifyPhone();

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.IsPhoneVerified.Should().BeTrue();
        user.UpdatedAtUtc.Should().BeOnOrAfter(user.CreatedAtUtc);
    }

    [Fact]
    public void VerifyPhone_WhenAlreadyVerified_ShouldFail()
    {
        // Arrange
        var user = User.Create(_validPhone, ValidPasswordHash, ValidFirstName, ValidSecondName, ValidLastName, UserType.Student, isPhoneVerified: true).Value;

        // Act
        var result = user.VerifyPhone();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(ErrorCodes.User.AlreadyVerified);
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public void UpdatePassword_WithValidPasswordHash_ShouldSucceed()
    {
        // Arrange
        var user = User.Create(_validPhone, ValidPasswordHash, ValidFirstName, ValidSecondName, ValidLastName, UserType.Student).Value;
        const string newPasswordHash = "new_hashed_password";

        // Act
        var result = user.UpdatePassword(newPasswordHash);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.PasswordHash.Should().Be(newPasswordHash);
        user.UpdatedAtUtc.Should().BeOnOrAfter(user.CreatedAtUtc);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void UpdatePassword_WithInvalidPasswordHash_ShouldFail(string newPasswordHash)
    {
        // Arrange
        var user = User.Create(_validPhone, ValidPasswordHash, ValidFirstName, ValidSecondName, ValidLastName, UserType.Student).Value;

        // Act
        var result = user.UpdatePassword(newPasswordHash);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(ErrorCodes.Validation.Required);
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public void UpdateProfile_WithValidData_ShouldSucceed()
    {
        // Arrange
        var user = User.Create(_validPhone, ValidPasswordHash, ValidFirstName, ValidSecondName, ValidLastName, UserType.Student).Value;
        const string newFirstName = "Mohamed";
        const string newSecondName = "Ahmed";
        const string newLastName = "Ali";
        const string email = "test@example.com";

        // Act
        var result = user.UpdateProfile(newFirstName, newSecondName, newLastName, email);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.FirstName.Should().Be(newFirstName);
        user.SecondName.Should().Be(newSecondName);
        user.LastName.Should().Be(newLastName);
        user.Email.Should().Be(email);
        user.UpdatedAtUtc.Should().BeOnOrAfter(user.CreatedAtUtc);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void UpdateProfile_WithInvalidFirstName_ShouldFail(string newFirstName)
    {
        // Arrange
        var user = User.Create(_validPhone, ValidPasswordHash, ValidFirstName, ValidSecondName, ValidLastName, UserType.Student).Value;

        // Act
        var result = user.UpdateProfile(newFirstName, ValidSecondName, ValidLastName);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(ErrorCodes.Validation.Required);
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public void UpdateProfile_WithTooLongFirstName_ShouldFail()
    {
        // Arrange
        var user = User.Create(_validPhone, ValidPasswordHash, ValidFirstName, ValidSecondName, ValidLastName, UserType.Student).Value;
        var longName = new string('A', 101);

        // Act
        var result = user.UpdateProfile(longName, ValidSecondName, ValidLastName);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(ErrorCodes.Validation.InvalidInput);
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public void Deactivate_WhenActive_ShouldSucceed()
    {
        // Arrange
        var user = User.Create(_validPhone, ValidPasswordHash, ValidFirstName, ValidSecondName, ValidLastName, UserType.Student).Value;
        const string reason = "Account suspended";

        // Act
        var result = user.Deactivate(reason);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.IsActive.Should().BeFalse();
        user.UpdatedAtUtc.Should().BeOnOrAfter(user.CreatedAtUtc);
    }

    [Fact]
    public void Deactivate_WhenAlreadyInactive_ShouldFail()
    {
        // Arrange
        var user = User.Create(_validPhone, ValidPasswordHash, ValidFirstName, ValidSecondName, ValidLastName, UserType.Student).Value;
        user.Deactivate("First deactivation");

        // Act
        var result = user.Deactivate("Second deactivation");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(ErrorCodes.User.Inactive);
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public void Reactivate_WhenInactive_ShouldSucceed()
    {
        // Arrange
        var user = User.Create(_validPhone, ValidPasswordHash, ValidFirstName, ValidSecondName, ValidLastName, UserType.Student).Value;
        user.Deactivate("Test");

        // Act
        var result = user.Reactivate();

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.IsActive.Should().BeTrue();
        user.UpdatedAtUtc.Should().BeOnOrAfter(user.CreatedAtUtc);
    }

    [Fact]
    public void Reactivate_WhenAlreadyActive_ShouldFail()
    {
        // Arrange
        var user = User.Create(_validPhone, ValidPasswordHash, ValidFirstName, ValidSecondName, ValidLastName, UserType.Student).Value;

        // Act
        var result = user.Reactivate();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(ErrorCodes.Validation.InvalidInput);
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public void CanLogin_WhenActiveAndVerified_ShouldSucceed()
    {
        // Arrange
        var user = User.Create(_validPhone, ValidPasswordHash, ValidFirstName, ValidSecondName, ValidLastName, UserType.Student, isPhoneVerified: true).Value;

        // Act
        var result = user.CanLogin();

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void CanLogin_WhenInactive_ShouldFail()
    {
        // Arrange
        var user = User.Create(_validPhone, ValidPasswordHash, ValidFirstName, ValidSecondName, ValidLastName, UserType.Student, isPhoneVerified: true).Value;
        user.Deactivate("Test");

        // Act
        var result = user.CanLogin();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(ErrorCodes.User.Inactive);
        result.Error.Type.Should().Be(ErrorType.Forbidden);
    }

    [Fact]
    public void CanLogin_WhenNotVerified_ShouldFail()
    {
        // Arrange
        var user = User.Create(_validPhone, ValidPasswordHash, ValidFirstName, ValidSecondName, ValidLastName, UserType.Student, isPhoneVerified: false).Value;

        // Act
        var result = user.CanLogin();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(ErrorCodes.User.NotVerified);
        result.Error.Type.Should().Be(ErrorType.Forbidden);
    }
}
