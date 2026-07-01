using Ambev.DeveloperEvaluation.Application.Sales.Common;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSaleItem;

public class UpdateSaleItemCommand : IRequest<SaleResult>
{
    public Guid SaleId { get; set; }
    public Guid SaleItemId { get; set; }
    public int NewQuantity { get; set; }
}
