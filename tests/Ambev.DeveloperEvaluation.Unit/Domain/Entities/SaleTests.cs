using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentValidation;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Contains unit tests for the Sale entity class.
/// Tests cover item management, cancellation, total recalculation and validation scenarios.
/// </summary>
public class SaleTests
{
    /// <summary>
    /// Tests that when an active sale is cancelled, its status changes to Cancelled.
    /// </summary>
    [Fact(DisplayName = "Sale status should change to Cancelled when cancelled")]
    public void Given_ActiveSale_When_Cancelled_Then_StatusShouldBeCancelled()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act
        sale.Cancel();

        // Assert
        Assert.Equal(SaleStatus.Cancelled, sale.Status);
        Assert.NotNull(sale.UpdatedAt);
    }

    /// <summary>
    /// Tests that cancelling a sale that is already cancelled throws a DomainException.
    /// </summary>
    [Fact(DisplayName = "Cancelling an already cancelled sale should throw DomainException")]
    public void Given_CancelledSale_When_CancelledAgain_Then_ShouldThrowDomainException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.Cancel();

        // Act & Assert
        Assert.Throws<DomainException>(() => sale.Cancel());
    }

    /// <summary>
    /// Tests that adding a valid item to a sale includes it in the sale items and recalculates the total.
    /// </summary>
    [Fact(DisplayName = "Adding a valid item should include it in the sale and recalculate the total")]
    public void Given_ValidSale_When_ItemAdded_Then_SaleItemsAndTotalShouldBeUpdated()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var productId = SaleItemTestData.GenerateValidProductId();
        var productName = SaleItemTestData.GenerateValidProductName();
        var quantity = SaleItemTestData.GenerateQuantityWithoutDiscount();
        var unitPrice = SaleItemTestData.GenerateValidUnitPrice();

        // Act
        sale.AddItem(productId, productName, quantity, unitPrice);

        // Assert
        Assert.Single(sale.SaleItems);
        Assert.Equal(unitPrice * quantity, sale.TotalAmount);
        Assert.NotNull(sale.UpdatedAt);
    }

    /// <summary>
    /// Tests that adding multiple items with different discount tiers correctly accumulates the sale total.
    /// </summary>
    [Fact(DisplayName = "Adding multiple items should correctly calculate the sale total")]
    public void Given_ValidSale_When_MultipleItemsAdded_Then_TotalAmountShouldBeSumOfItemTotals()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        var unitPrice1 = SaleItemTestData.GenerateValidUnitPrice();
        var quantity1 = SaleItemTestData.GenerateQuantityWithoutDiscount();

        var unitPrice2 = SaleItemTestData.GenerateValidUnitPrice();
        var quantity2 = SaleItemTestData.GenerateQuantityWithTenPercentDiscount();

        var unitPrice3 = SaleItemTestData.GenerateValidUnitPrice();
        var quantity3 = SaleItemTestData.GenerateQuantityWithTwentyPercentDiscount();

        // Act
        sale.AddItem(SaleItemTestData.GenerateValidProductId(), SaleItemTestData.GenerateValidProductName(), quantity1, unitPrice1);
        sale.AddItem(SaleItemTestData.GenerateValidProductId(), SaleItemTestData.GenerateValidProductName(), quantity2, unitPrice2);
        sale.AddItem(SaleItemTestData.GenerateValidProductId(), SaleItemTestData.GenerateValidProductName(), quantity3, unitPrice3);

        // Assert
        var expectedTotal = sale.SaleItems.Sum(i => i.TotalAmount);
        Assert.Equal(3, sale.SaleItems.Count);
        Assert.Equal(unitPrice1 * quantity1, sale.SaleItems.ElementAt(0).TotalAmount);
        Assert.Equal(unitPrice2 * quantity2 * 0.90m, sale.SaleItems.ElementAt(1).TotalAmount);
        Assert.Equal(unitPrice3 * quantity3 * 0.80m, sale.SaleItems.ElementAt(2).TotalAmount);
        Assert.Equal(expectedTotal, sale.TotalAmount);
    }

    /// <summary>
    /// Tests that adding a product that already exists in the sale throws a DomainException.
    /// </summary>
    [Fact(DisplayName = "Adding a duplicate product should throw DomainException")]
    public void Given_SaleWithExistingProduct_When_SameProductAddedAgain_Then_ShouldThrowDomainException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var productId = SaleItemTestData.GenerateValidProductId();
        sale.AddItem(productId, SaleItemTestData.GenerateValidProductName(), SaleItemTestData.GenerateQuantityWithoutDiscount(), SaleItemTestData.GenerateValidUnitPrice());

        // Act & Assert
        Assert.Throws<DomainException>(() => sale.AddItem(
            productId,
            SaleItemTestData.GenerateValidProductName(),
            SaleItemTestData.GenerateQuantityWithoutDiscount(),
            SaleItemTestData.GenerateValidUnitPrice()));
    }

    /// <summary>
    /// Tests that adding an item to a cancelled sale throws a DomainException.
    /// </summary>
    [Fact(DisplayName = "Adding an item to a cancelled sale should throw DomainException")]
    public void Given_CancelledSale_When_ItemAdded_Then_ShouldThrowDomainException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.Cancel();

        // Act & Assert
        Assert.Throws<DomainException>(() => sale.AddItem(
            SaleItemTestData.GenerateValidProductId(),
            SaleItemTestData.GenerateValidProductName(),
            SaleItemTestData.GenerateQuantityWithoutDiscount(),
            SaleItemTestData.GenerateValidUnitPrice()));
    }

    /// <summary>
    /// Tests that adding an item with invalid data throws a ValidationException.
    /// </summary>
    [Fact(DisplayName = "Adding an item with invalid data should throw ValidationException")]
    public void Given_ValidSale_When_InvalidItemAdded_Then_ShouldThrowValidationException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act & Assert
        Assert.Throws<ValidationException>(() => sale.AddItem(
            SaleItemTestData.GenerateValidProductId(),
            SaleItemTestData.GenerateValidProductName(),
            SaleItemTestData.GenerateQuantityAboveLimit(),
            SaleItemTestData.GenerateValidUnitPrice()));
    }

    /// <summary>
    /// Tests that updating the quantity of an existing item recalculates the sale total.
    /// </summary>
    [Fact(DisplayName = "Updating an item quantity should recalculate the sale total")]
    public void Given_SaleWithItem_When_ItemQuantityUpdated_Then_TotalShouldBeRecalculated()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var unitPrice = SaleItemTestData.GenerateValidUnitPrice();
        sale.AddItem(SaleItemTestData.GenerateValidProductId(), SaleItemTestData.GenerateValidProductName(), 1, unitPrice);
        var itemId = sale.SaleItems.Single().Id;

        // Act
        sale.UpdateItemQuantity(itemId, 15);

        // Assert
        Assert.Equal(unitPrice * 15 * 0.80m, sale.TotalAmount);
    }

    /// <summary>
    /// Tests that updating the quantity of a non-existing item throws a DomainException.
    /// </summary>
    [Fact(DisplayName = "Updating a non-existing item quantity should throw DomainException")]
    public void Given_SaleWithoutItem_When_ItemQuantityUpdated_Then_ShouldThrowDomainException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act & Assert
        Assert.Throws<DomainException>(() => sale.UpdateItemQuantity(Guid.NewGuid(), 5));
    }

    /// <summary>
    /// Tests that updating an item quantity on a cancelled sale throws a DomainException.
    /// </summary>
    [Fact(DisplayName = "Updating an item quantity on a cancelled sale should throw DomainException")]
    public void Given_CancelledSale_When_ItemQuantityUpdated_Then_ShouldThrowDomainException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSaleWithItems();
        var itemId = sale.SaleItems.Single().Id;
        sale.Cancel();

        // Act & Assert
        Assert.Throws<DomainException>(() => sale.UpdateItemQuantity(itemId, 5));
    }

    /// <summary>
    /// Tests that cancelling an item excludes it from the sale total.
    /// </summary>
    [Fact(DisplayName = "Cancelling an item should exclude it from the sale total")]
    public void Given_SaleWithItem_When_ItemCancelled_Then_TotalShouldExcludeCancelledItem()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.AddItem(SaleItemTestData.GenerateValidProductId(), SaleItemTestData.GenerateValidProductName(), SaleItemTestData.GenerateQuantityWithoutDiscount(), SaleItemTestData.GenerateValidUnitPrice());
        var itemId = sale.SaleItems.Single().Id;

        // Act
        sale.CancelItem(itemId);

        // Assert
        Assert.True(sale.SaleItems.Single().IsCancelled);
        Assert.Equal(0m, sale.TotalAmount);
    }

    /// <summary>
    /// Tests that cancelling a non-existing item throws a DomainException.
    /// </summary>
    [Fact(DisplayName = "Cancelling a non-existing item should throw DomainException")]
    public void Given_SaleWithoutItem_When_ItemCancelled_Then_ShouldThrowDomainException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act & Assert
        Assert.Throws<DomainException>(() => sale.CancelItem(Guid.NewGuid()));
    }

    /// <summary>
    /// Tests that cancelling an item on a cancelled sale throws a DomainException.
    /// </summary>
    [Fact(DisplayName = "Cancelling an item on a cancelled sale should throw DomainException")]
    public void Given_CancelledSale_When_ItemCancelled_Then_ShouldThrowDomainException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSaleWithItems();
        var itemId = sale.SaleItems.Single().Id;
        sale.Cancel();

        // Act & Assert
        Assert.Throws<DomainException>(() => sale.CancelItem(itemId));
    }

    /// <summary>
    /// Tests that removing an item from the sale excludes it from the sale items and recalculates the total.
    /// </summary>
    [Fact(DisplayName = "Removing an item should remove it from the sale and recalculate the total")]
    public void Given_SaleWithItem_When_ItemRemoved_Then_SaleItemsAndTotalShouldBeUpdated()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.AddItem(SaleItemTestData.GenerateValidProductId(), SaleItemTestData.GenerateValidProductName(), SaleItemTestData.GenerateQuantityWithoutDiscount(), SaleItemTestData.GenerateValidUnitPrice());
        var itemId = sale.SaleItems.Single().Id;

        // Act
        sale.RemoveItem(itemId);

        // Assert
        Assert.Empty(sale.SaleItems);
        Assert.Equal(0m, sale.TotalAmount);
    }

    /// <summary>
    /// Tests that removing a non-existing item throws a DomainException.
    /// </summary>
    [Fact(DisplayName = "Removing a non-existing item should throw DomainException")]
    public void Given_SaleWithoutItem_When_ItemRemoved_Then_ShouldThrowDomainException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act & Assert
        Assert.Throws<DomainException>(() => sale.RemoveItem(Guid.NewGuid()));
    }

    /// <summary>
    /// Tests that removing an item on a cancelled sale throws a DomainException.
    /// </summary>
    [Fact(DisplayName = "Removing an item on a cancelled sale should throw DomainException")]
    public void Given_CancelledSale_When_ItemRemoved_Then_ShouldThrowDomainException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSaleWithItems();
        var itemId = sale.SaleItems.Single().Id;
        sale.Cancel();

        // Act & Assert
        Assert.Throws<DomainException>(() => sale.RemoveItem(itemId));
    }

    /// <summary>
    /// Tests that validation passes when all sale properties are valid.
    /// </summary>
    [Fact(DisplayName = "Validation should pass for valid sale data")]
    public void Given_ValidSaleData_When_Validated_Then_ShouldReturnValid()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act
        var result = sale.Validate();

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    /// <summary>
    /// Tests that validation fails when sale properties are invalid.
    /// </summary>
    [Fact(DisplayName = "Validation should fail for invalid sale data")]
    public void Given_InvalidSaleData_When_Validated_Then_ShouldReturnInvalid()
    {
        // Arrange
        var sale = SaleTestData.GenerateInvalidSale();

        // Act
        var result = sale.Validate();

        // Assert
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }
}
