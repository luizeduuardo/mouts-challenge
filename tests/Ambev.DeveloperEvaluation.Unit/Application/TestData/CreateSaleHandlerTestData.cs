using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class CreateSaleHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid sale item commands.
    /// The generated items will have valid:
    /// - ProductId and ProductName
    /// - UnitPrice (greater than 0)
    /// - Quantity (between 1 and 20, respecting the maximum allowed per item)
    /// </summary>
    private static readonly Faker<SaleItemCommand> saleItemCommandFaker = new Faker<SaleItemCommand>()
        .RuleFor(i => i.ProductId, f => f.Random.Guid())
        .RuleFor(i => i.ProductName, f => f.Commerce.ProductName())
        .RuleFor(i => i.UnitPrice, f => f.Random.Decimal(1, 500))
        .RuleFor(i => i.Quantity, f => f.Random.Int(1, 20));

    /// <summary>
    /// Configures the Faker to generate valid sale commands.
    /// The generated commands will have valid:
    /// - CustomerId and CustomerName
    /// - BranchId and BranchName
    /// - SaleItems (a non-empty collection of valid items)
    /// </summary>
    private static readonly Faker<CreateSaleCommand> createSaleCommandFaker = new Faker<CreateSaleCommand>()
        .RuleFor(s => s.CustomerId, f => f.Random.Guid())
        .RuleFor(s => s.CustomerName, f => f.Person.FullName)
        .RuleFor(s => s.BranchId, f => f.Random.Guid())
        .RuleFor(s => s.BranchName, f => f.Company.CompanyName())
        .RuleFor(s => s.SaleItems, f => saleItemCommandFaker.Generate(2));

    /// <summary>
    /// Generates a valid CreateSaleCommand with randomized data and the given number of items.
    /// </summary>
    /// <param name="itemCount">The number of sale items to generate.</param>
    /// <returns>A valid CreateSaleCommand with randomly generated data.</returns>
    public static CreateSaleCommand GenerateValidCommand(int itemCount = 2)
    {
        var command = createSaleCommandFaker.Generate();
        command.SaleItems = saleItemCommandFaker.Generate(itemCount);
        return command;
    }
}
