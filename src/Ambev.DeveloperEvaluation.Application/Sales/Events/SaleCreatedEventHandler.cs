using Ambev.DeveloperEvaluation.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.Events;

public class SaleCreatedEventHandler : INotificationHandler<SaleCreatedEvent>
{
    private readonly ILogger<SaleCreatedEventHandler> _logger;

    public SaleCreatedEventHandler(ILogger<SaleCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(SaleCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Sale created: {SaleId} (#{SaleNumber}) for customer {CustomerId} at branch {BranchId}, total {TotalAmount}",
            notification.SaleId, notification.SaleNumber, notification.CustomerId, notification.BranchId, notification.TotalAmount);

        return Task.CompletedTask;
    }
}
