using FluentAssertions;
using LMS.Domain.Common;

namespace LMS.Domain.Tests.Common;

public sealed class MoneyTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(10.50)]
    [InlineData(100)]
    [InlineData(1000.99)]
    [InlineData(0.01)]
    public void Create_WithValidAmount_ShouldSucceed(decimal amount)
    {
        // Act
        var result = Money.Create(amount);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Amount.Should().Be(amount);
        result.Value.Currency.Should().Be("EGP");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10.50)]
    [InlineData(-100)]
    public void Create_WithNegativeAmount_ShouldFail(decimal amount)
    {
        // Act
        var result = Money.Create(amount);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("VALIDATION.INVALID_INPUT");
        result.Error.Message.Should().Contain("Amount cannot be negative");
    }

    [Fact]
    public void Add_TwoMoneyObjects_ShouldReturnSum()
    {
        // Arrange
        var money1 = Money.Create(10).Value;
        var money2 = Money.Create(20).Value;

        // Act
        var result = money1 + money2;

        // Assert
        result.Amount.Should().Be(30);
    }

    [Fact]
    public void Subtract_TwoMoneyObjects_ShouldReturnDifference()
    {
        // Arrange
        var money1 = Money.Create(50).Value;
        var money2 = Money.Create(20).Value;

        // Act
        var result = money1 - money2;

        // Assert
        result.Amount.Should().Be(30);
    }

    [Fact]
    public void Subtract_ResultingInNegative_ShouldReturnNegative()
    {
        // Arrange
        var money1 = Money.Create(20).Value;
        var money2 = Money.Create(50).Value;

        // Act
        var result = money1 - money2;

        // Assert
        result.Amount.Should().Be(-30);
    }

    [Theory]
    [InlineData(10, 2, 20)]
    [InlineData(15.50, 3, 46.50)]
    [InlineData(100, 0.5, 50)]
    public void Multiply_ByMultiplier_ShouldReturnProduct(decimal amount, decimal multiplier, decimal expected)
    {
        // Arrange
        var money = Money.Create(amount).Value;

        // Act
        var result = money * multiplier;

        // Assert
        result.Amount.Should().Be(expected);
    }

    [Fact]
    public void Multiply_ByNegativeMultiplier_ShouldReturnNegative()
    {
        // Arrange
        var money = Money.Create(100).Value;

        // Act
        var result = money * -2;

        // Assert
        result.Amount.Should().Be(-200);
    }

    [Fact]
    public void Equals_WithSameAmount_ShouldBeEqual()
    {
        // Arrange
        var money1 = Money.Create(100).Value;
        var money2 = Money.Create(100).Value;

        // Act & Assert
        money1.Should().Be(money2);
        (money1 == money2).Should().BeTrue();
        money1.Equals(money2).Should().BeTrue();
        money1.GetHashCode().Should().Be(money2.GetHashCode());
    }

    [Fact]
    public void Equals_WithDifferentAmount_ShouldNotBeEqual()
    {
        // Arrange
        var money1 = Money.Create(100).Value;
        var money2 = Money.Create(200).Value;

        // Act & Assert
        money1.Should().NotBe(money2);
        (money1 == money2).Should().BeFalse();
        (money1 != money2).Should().BeTrue();
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var money = Money.Create(100.50m).Value;

        // Act
        var moneyString = money.ToString();

        // Assert
        moneyString.Should().Be("100.50 EGP");
    }

    [Fact]
    public void Zero_ShouldReturnZeroMoney()
    {
        // Act
        var zero = Money.Zero();

        // Assert
        zero.Amount.Should().Be(0);
        zero.Currency.Should().Be("EGP");
    }
}
