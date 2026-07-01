using Ambev.DeveloperEvaluation.Application.Sales.Events;
using Ambev.DeveloperEvaluation.Domain.Events;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class SaleEventHandlersTests
{
    [Fact(DisplayName = "SaleCreatedEventHandler logs without throwing")]
    public async Task SaleCreatedEventHandler_Handle_DoesNotThrow()
    {
        var handler = new SaleCreatedEventHandler(NullLogger<SaleCreatedEventHandler>.Instance);
        var notification = new SaleCreatedEvent(Guid.NewGuid(), 1234, Guid.NewGuid(), "Customer", Guid.NewGuid(), "Branch", 100m, DateTime.UtcNow);

        var act = () => handler.Handle(notification, CancellationToken.None);

        await act.Should().NotThrowAsync();
    }

    [Fact(DisplayName = "SaleModifiedEventHandler logs without throwing")]
    public async Task SaleModifiedEventHandler_Handle_DoesNotThrow()
    {
        var handler = new SaleModifiedEventHandler(NullLogger<SaleModifiedEventHandler>.Instance);
        var notification = new SaleModifiedEvent(Guid.NewGuid(), 1234, 100m, DateTime.UtcNow);

        var act = () => handler.Handle(notification, CancellationToken.None);

        await act.Should().NotThrowAsync();
    }

    [Fact(DisplayName = "SaleCancelledEventHandler logs without throwing")]
    public async Task SaleCancelledEventHandler_Handle_DoesNotThrow()
    {
        var handler = new SaleCancelledEventHandler(NullLogger<SaleCancelledEventHandler>.Instance);
        var notification = new SaleCancelledEvent(Guid.NewGuid(), 1234, DateTime.UtcNow);

        var act = () => handler.Handle(notification, CancellationToken.None);

        await act.Should().NotThrowAsync();
    }

    [Fact(DisplayName = "ItemCancelledEventHandler logs without throwing")]
    public async Task ItemCancelledEventHandler_Handle_DoesNotThrow()
    {
        var handler = new ItemCancelledEventHandler(NullLogger<ItemCancelledEventHandler>.Instance);
        var notification = new ItemCancelledEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Product", DateTime.UtcNow);

        var act = () => handler.Handle(notification, CancellationToken.None);

        await act.Should().NotThrowAsync();
    }
}
