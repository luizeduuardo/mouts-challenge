using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

/// <summary>
/// Provides methods for generating test data for SaleItem-related values using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class SaleItemTestData
{
    /// <summary>
    /// Generates a valid product id.
    /// </summary>
    /// <returns>A valid, non-empty product id.</returns>
    public static Guid GenerateValidProductId()
    {
        return Guid.NewGuid();
    }

    /// <summary>
    /// Generates a valid product name.
    /// </summary>
    /// <returns>A valid, non-empty product name.</returns>
    public static string GenerateValidProductName()
    {
        return new Faker().Commerce.ProductName();
    }

    /// <summary>
    /// Generates a valid unit price greater than zero.
    /// </summary>
    /// <returns>A valid unit price.</returns>
    public static decimal GenerateValidUnitPrice()
    {
        return new Faker().Random.Decimal(1, 500);
    }

    /// <summary>
    /// Generates a valid quantity that does not trigger any discount tier (below 4 items).
    /// </summary>
    /// <returns>A valid quantity between 1 and 3.</returns>
    public static int GenerateQuantityWithoutDiscount()
    {
        return new Faker().Random.Number(1, 3);
    }

    /// <summary>
    /// Generates a valid quantity that falls within the 10% discount tier (4 to 9 items).
    /// </summary>
    /// <returns>A valid quantity between 4 and 9.</returns>
    public static int GenerateQuantityWithTenPercentDiscount()
    {
        return new Faker().Random.Number(4, 9);
    }

    /// <summary>
    /// Generates a valid quantity that falls within the 20% discount tier (10 to 20 items).
    /// </summary>
    /// <returns>A valid quantity between 10 and 20.</returns>
    public static int GenerateQuantityWithTwentyPercentDiscount()
    {
        return new Faker().Random.Number(10, 20);
    }

    /// <summary>
    /// Generates a quantity that exceeds the maximum allowed items per product (above 20).
    /// </summary>
    /// <returns>An invalid quantity greater than 20.</returns>
    public static int GenerateQuantityAboveLimit()
    {
        return new Faker().Random.Number(21, 100);
    }
}
