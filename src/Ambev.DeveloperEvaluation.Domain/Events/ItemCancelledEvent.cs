using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class ItemCancelledEvent : INotification
{
    public Guid SaleId { get; }
    public Guid SaleItemId { get; }
    public Guid ProductId { get; }
    public string ProductName { get; }
    public DateTime? CancelledAt { get; }

    public ItemCancelledEvent(Guid saleId, Guid saleItemId, Guid productId, string productName, DateTime? cancelledAt)
    {
        SaleId = saleId;
        SaleItemId = saleItemId;
        ProductId = productId;
        ProductName = productName;
        CancelledAt = cancelledAt;
    }
}
