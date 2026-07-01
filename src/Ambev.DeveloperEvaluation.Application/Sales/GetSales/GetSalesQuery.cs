using Ambev.DeveloperEvaluation.Domain.Enums;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;

public record GetSalesQuery : IRequest<GetSalesResult>
{
    public Guid? CustomerId { get; set; }
    public Guid? BranchId { get; set; }
    public SaleStatus? Status { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
