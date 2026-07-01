using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using MediatR;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class MediatorDomainEventExtensionsTests
{
    private readonly IMediator _mediator;

    public MediatorDomainEventExtensionsTests()
    {
        _mediator = Substitute.For<IMediator>();
    }

    [Fact(DisplayName = "Given an entity with queued events When dispatching Then publishes each event and clears the queue")]
    public async Task DispatchDomainEventsAsync_EntityWithQueuedEvents_PublishesAndClearsQueue()
    {
        // Given
        var sale = SaleTestData.GenerateValidSale();
        sale.Cancel(); // queues a SaleCancelledEvent

        // When
        await _mediator.DispatchDomainEventsAsync(sale, CancellationToken.None);

        // Then
        await _mediator.Received(1).Publish(Arg.Is<object>(o => o is SaleCancelledEvent), Arg.Any<CancellationToken>());
        Assert.Empty(sale.DomainEvents);
    }

    [Fact(DisplayName = "Given an entity with no queued events When dispatching Then publishes nothing")]
    public async Task DispatchDomainEventsAsync_EntityWithNoEvents_PublishesNothing()
    {
        // Given
        var sale = SaleTestData.GenerateValidSale();

        // When
        await _mediator.DispatchDomainEventsAsync(sale, CancellationToken.None);

        // Then
        await _mediator.DidNotReceive().Publish(Arg.Any<object>(), Arg.Any<CancellationToken>());
    }
}
