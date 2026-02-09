using FluentAssertions;
using LMS.Domain.Common;

namespace LMS.Domain.Tests.Common;

public sealed class PhoneTests
{
    [Theory]
    [InlineData("01012345678")] // Vodafone
    [InlineData("01112345678")] // Etisalat
    [InlineData("01212345678")] // Orange
    [InlineData("01512345678")] // WE
    public void Create_WithValidEgyptianPhone_ShouldSucceed(string phoneNumber)
    {
        // Act
        var result = Phone.Create(phoneNumber);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(phoneNumber);
    }

    [Theory]
    [InlineData("")] // Empty
    [InlineData(" ")] // Whitespace
    [InlineData("123456")] // Too short
    [InlineData("0101234567")] // 10 digits instead of 11
    [InlineData("010123456789")] // 12 digits
    [InlineData("02012345678")] // Doesn't start with 01
    [InlineData("01312345678")] // Invalid network code (013 doesn't exist)
    [InlineData("1012345678")] // Missing leading 0
    [InlineData("abc12345678")] // Contains letters
    public void Create_WithInvalidPhone_ShouldFail(string phoneNumber)
    {
        // Act
        var result = Phone.Create(phoneNumber);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(ErrorCodes.User.InvalidPhone);
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Theory]
    [InlineData("+201012345678", "01012345678")] // International format with +20 is cleaned
    [InlineData("201012345678", "01012345678")] // International format without + is cleaned
    [InlineData("01 01 234 5678", "01012345678")] // Spaces are cleaned
    [InlineData("010-1234-5678", "01012345678")] // Dashes are cleaned
    public void Create_WithCleanableFormat_ShouldSucceed(string phoneNumber, string expected)
    {
        // Act
        var result = Phone.Create(phoneNumber);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(expected);
    }

    [Fact]
    public void Create_WithNull_ShouldFail()
    {
        // Act
        var result = Phone.Create(null!);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(ErrorCodes.User.InvalidPhone);
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public void Equals_WithSamePhoneNumber_ShouldBeEqual()
    {
        // Arrange
        var phone1 = Phone.Create("01012345678").Value;
        var phone2 = Phone.Create("01012345678").Value;

        // Act & Assert
        phone1.Should().Be(phone2);
        (phone1 == phone2).Should().BeTrue();
        phone1.Equals(phone2).Should().BeTrue();
        phone1.GetHashCode().Should().Be(phone2.GetHashCode());
    }

    [Fact]
    public void Equals_WithDifferentPhoneNumber_ShouldNotBeEqual()
    {
        // Arrange
        var phone1 = Phone.Create("01012345678").Value;
        var phone2 = Phone.Create("01112345678").Value;

        // Act & Assert
        phone1.Should().NotBe(phone2);
        (phone1 == phone2).Should().BeFalse();
        (phone1 != phone2).Should().BeTrue();
    }

    [Fact]
    public void ToString_ShouldReturnPhoneNumber()
    {
        // Arrange
        var phone = Phone.Create("01012345678").Value;

        // Act
        var phoneString = phone.ToString();

        // Assert
        phoneString.Should().Be("01012345678");
    }
}
