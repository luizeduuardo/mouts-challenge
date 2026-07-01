namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.RemoveSaleItem;

public class RemoveSaleItemRequest
{
    public Guid SaleId { get; set; }
    public Guid SaleItemId { get; set; }
}
