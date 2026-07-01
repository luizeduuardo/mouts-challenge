using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Contains unit tests for the SaleItem entity class.
/// Tests cover discount calculation, quantity updates, cancellation and validation scenarios.
/// </summary>
public class SaleItemTests
{
    /// <summary>
    /// Tests that creating a sale item with quantities below 4 applies no discount.
    /// </summary>
    [Theory(DisplayName = "SaleItem should have no discount when quantity is below 4")]
    [InlineData(1)]
    [InlineData(3)]
    public void Given_QuantityBelowFour_When_Created_Then_NoDiscountShouldBeApplied(int quantity)
    {
        // Arrange
        var unitPrice = SaleItemTestData.GenerateValidUnitPrice();

        // Act
        var item = new SaleItem(SaleItemTestData.GenerateValidProductId(), SaleItemTestData.GenerateValidProductName(), unitPrice, quantity);

        // Assert
        Assert.Equal(0m, item.Discount);
        Assert.Equal(unitPrice * quantity, item.TotalAmount);
    }

    /// <summary>
    /// Tests that creating a sale item with quantities between 4 and 9 applies a 10% discount.
    /// </summary>
    [Theory(DisplayName = "SaleItem should have a 10% discount when quantity is between 4 and 9")]
    [InlineData(4)]
    [InlineData(9)]
    public void Given_QuantityBetweenFourAndNine_When_Created_Then_TenPercentDiscountShouldBeApplied(int quantity)
    {
        // Arrange
        var unitPrice = SaleItemTestData.GenerateValidUnitPrice();

        // Act
        var item = new SaleItem(SaleItemTestData.GenerateValidProductId(), SaleItemTestData.GenerateValidProductName(), unitPrice, quantity);

        // Assert
        Assert.Equal(0.10m, item.Discount);
        Assert.Equal(unitPrice * quantity * 0.90m, item.TotalAmount);
    }

    /// <summary>
    /// Tests that creating a sale item with quantities between 10 and 20 applies a 20% discount.
    /// </summary>
    [Theory(DisplayName = "SaleItem should have a 20% discount when quantity is between 10 and 20")]
    [InlineData(10)]
    [InlineData(20)]
    public void Given_QuantityBetweenTenAndTwenty_When_Created_Then_TwentyPercentDiscountShouldBeApplied(int quantity)
    {
        // Arrange
        var unitPrice = SaleItemTestData.GenerateValidUnitPrice();

        // Act
        var item = new SaleItem(SaleItemTestData.GenerateValidProductId(), SaleItemTestData.GenerateValidProductName(), unitPrice, quantity);

        // Assert
        Assert.Equal(0.20m, item.Discount);
        Assert.Equal(unitPrice * quantity * 0.80m, item.TotalAmount);
    }

    /// <summary>
    /// Tests that updating the quantity of a sale item recalculates the discount and total amount.
    /// </summary>
    [Fact(DisplayName = "SaleItem discount and total should be recalculated when quantity is updated")]
    public void Given_ExistingSaleItem_When_QuantityUpdated_Then_DiscountAndTotalShouldBeRecalculated()
    {
        // Arrange
        var unitPrice = SaleItemTestData.GenerateValidUnitPrice();
        var item = new SaleItem(SaleItemTestData.GenerateValidProductId(), SaleItemTestData.GenerateValidProductName(), unitPrice, SaleItemTestData.GenerateQuantityWithoutDiscount());

        // Act
        item.UpdateQuantity(15);

        // Assert
        Assert.Equal(15, item.Quantity);
        Assert.Equal(0.20m, item.Discount);
        Assert.Equal(unitPrice * 15 * 0.80m, item.TotalAmount);
    }

    /// <summary>
    /// Tests that cancelling a sale item marks it as cancelled.
    /// </summary>
    [Fact(DisplayName = "SaleItem should be marked as cancelled when cancelled")]
    public void Given_ActiveSaleItem_When_Cancelled_Then_IsCancelledShouldBeTrue()
    {
        // Arrange
        var item = new SaleItem(
            SaleItemTestData.GenerateValidProductId(),
            SaleItemTestData.GenerateValidProductName(),
            SaleItemTestData.GenerateValidUnitPrice(),
            SaleItemTestData.GenerateQuantityWithoutDiscount());

        // Act
        item.Cancel();

        // Assert
        Assert.True(item.IsCancelled);
    }

    /// <summary>
    /// Tests that validation passes when all sale item properties are valid.
    /// </summary>
    [Fact(DisplayName = "Validation should pass for valid sale item data")]
    public void Given_ValidSaleItemData_When_Validated_Then_ShouldReturnValid()
    {
        // Arrange
        var item = new SaleItem(
            SaleItemTestData.GenerateValidProductId(),
            SaleItemTestData.GenerateValidProductName(),
            SaleItemTestData.GenerateValidUnitPrice(),
            SaleItemTestData.GenerateQuantityWithoutDiscount());

        // Act
        var result = item.Validate();

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    /// <summary>
    /// Tests that validation fails when sale item properties are invalid.
    /// </summary>
    [Fact(DisplayName = "Validation should fail for invalid sale item data")]
    public void Given_InvalidSaleItemData_When_Validated_Then_ShouldReturnInvalid()
    {
        // Arrange
        var item = new SaleItem(
            Guid.Empty, // Invalid: empty product id
            string.Empty, // Invalid: empty product name
            0m, // Invalid: unit price must be greater than 0
            0); // Invalid: quantity must be greater than 0

        // Act
        var result = item.Validate();

        // Assert
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }

    /// <summary>
    /// Tests that validation fails when quantity exceeds the maximum allowed items per product.
    /// </summary>
    [Fact(DisplayName = "Validation should fail when quantity exceeds the maximum limit")]
    public void Given_QuantityAboveLimit_When_Validated_Then_ShouldReturnInvalid()
    {
        // Arrange
        var item = new SaleItem(
            SaleItemTestData.GenerateValidProductId(),
            SaleItemTestData.GenerateValidProductName(),
            SaleItemTestData.GenerateValidUnitPrice(),
            SaleItemTestData.GenerateQuantityAboveLimit());

        // Act
        var result = item.Validate();

        // Assert
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }
}
