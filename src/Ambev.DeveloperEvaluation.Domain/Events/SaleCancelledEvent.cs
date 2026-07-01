using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class SaleCancelledEvent : INotification
{
    public Guid SaleId { get; }
    public int SaleNumber { get; }
    public DateTime? CancelledAt { get; }

    public SaleCancelledEvent(Guid saleId, int saleNumber, DateTime? cancelledAt)
    {
        SaleId = saleId;
        SaleNumber = saleNumber;
        CancelledAt = cancelledAt;
    }
}
