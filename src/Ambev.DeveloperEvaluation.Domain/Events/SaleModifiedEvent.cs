using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class SaleModifiedEvent : INotification
{
    public Guid SaleId { get; }
    public int SaleNumber { get; }
    public decimal TotalAmount { get; }
    public DateTime? UpdatedAt { get; }

    public SaleModifiedEvent(Guid saleId, int saleNumber, decimal totalAmount, DateTime? updatedAt)
    {
        SaleId = saleId;
        SaleNumber = saleNumber;
        TotalAmount = totalAmount;
        UpdatedAt = updatedAt;
    }
}
