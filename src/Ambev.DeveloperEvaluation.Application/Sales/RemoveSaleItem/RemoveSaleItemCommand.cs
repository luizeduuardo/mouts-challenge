using Ambev.DeveloperEvaluation.Application.Sales.Common;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.RemoveSaleItem;

public class RemoveSaleItemCommand : IRequest<SaleResult>
{
    public Guid SaleId { get; set; }
    public Guid SaleItemId { get; set; }
}
