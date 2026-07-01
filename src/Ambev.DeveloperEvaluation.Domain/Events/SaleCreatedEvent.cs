using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class SaleCreatedEvent : INotification
{
    public Guid SaleId { get; }
    public int SaleNumber { get; }
    public Guid CustomerId { get; }
    public string CustomerName { get; }
    public Guid BranchId { get; }
    public string BranchName { get; }
    public decimal TotalAmount { get; }
    public DateTime CreatedAt { get; }

    public SaleCreatedEvent(
        Guid saleId,
        int saleNumber,
        Guid customerId,
        string customerName,
        Guid branchId,
        string branchName,
        decimal totalAmount,
        DateTime createdAt)
    {
        SaleId = saleId;
        SaleNumber = saleNumber;
        CustomerId = customerId;
        CustomerName = customerName;
        BranchId = branchId;
        BranchName = branchName;
        TotalAmount = totalAmount;
        CreatedAt = createdAt;
    }
}
