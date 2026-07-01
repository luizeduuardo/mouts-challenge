using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class AddSaleItemHandlerTestData
{
    public static Guid GenerateValidProductId() => Guid.NewGuid();

    public static string GenerateValidProductName() => new Faker().Commerce.ProductName();

    public static decimal GenerateValidUnitPrice() => new Faker().Random.Decimal(1, 500);

    public static int GenerateValidQuantity() => new Faker().Random.Int(1, 20);

    public static int GenerateQuantityAboveLimit() => new Faker().Random.Int(21, 100);
}
