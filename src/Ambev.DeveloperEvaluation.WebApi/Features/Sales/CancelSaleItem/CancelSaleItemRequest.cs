namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSaleItem;

public class CancelSaleItemRequest
{
    public Guid SaleId { get; set; }
    public Guid SaleItemId { get; set; }
}
