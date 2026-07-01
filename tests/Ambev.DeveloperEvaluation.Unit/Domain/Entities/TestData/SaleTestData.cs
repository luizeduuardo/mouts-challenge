using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class SaleTestData
{
    /// <summary>
    /// Configures the Faker to generate valid Sale entities.
    /// The generated sales will have valid:
    /// - CustomerId and CustomerName
    /// - BranchId and BranchName
    /// - Status (always Active, so business rules can be exercised)
    /// </summary>
    private static readonly Faker<Sale> SaleFaker = new Faker<Sale>()
        .RuleFor(s => s.SaleNumber, f => f.Random.Number(1000, 9999))
        .RuleFor(s => s.CustomerId, f => f.Random.Guid())
        .RuleFor(s => s.CustomerName, f => f.Person.FullName)
        .RuleFor(s => s.BranchId, f => f.Random.Guid())
        .RuleFor(s => s.BranchName, f => f.Company.CompanyName())
        .RuleFor(s => s.Status, _ => SaleStatus.Active);

    /// <summary>
    /// Generates a valid Sale entity with randomized data and no items.
    /// </summary>
    /// <returns>A valid Sale entity with randomly generated data.</returns>
    public static Sale GenerateValidSale()
    {
        return SaleFaker.Generate();
    }

    /// <summary>
    /// Generates a valid Sale entity with the given number of valid items added to it.
    /// </summary>
    /// <param name="itemCount">The number of items to add to the sale.</param>
    /// <returns>A valid Sale entity containing the requested number of items.</returns>
    public static Sale GenerateValidSaleWithItems(int itemCount = 1)
    {
        var sale = GenerateValidSale();

        for (var i = 0; i < itemCount; i++)
        {
            sale.AddItem(
                SaleItemTestData.GenerateValidProductId(),
                SaleItemTestData.GenerateValidProductName(),
                SaleItemTestData.GenerateQuantityWithoutDiscount(),
                SaleItemTestData.GenerateValidUnitPrice());
        }

        return sale;
    }

    /// <summary>
    /// Generates a Sale entity with invalid data for testing negative validation scenarios.
    /// The generated sale will have empty CustomerId, CustomerName, BranchId and BranchName.
    /// </summary>
    /// <returns>An invalid Sale entity.</returns>
    public static Sale GenerateInvalidSale()
    {
        return new Sale
        {
            CustomerId = Guid.Empty,
            CustomerName = string.Empty,
            BranchId = Guid.Empty,
            BranchName = string.Empty
        };
    }
}
