using Ambev.DeveloperEvaluation.Domain.Common;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Common;

public static class MediatorDomainEventExtensions
{
    public static async Task DispatchDomainEventsAsync(this IMediator mediator, BaseEntity entity, CancellationToken cancellationToken = default)
    {
        var domainEvents = entity.DomainEvents.ToList();
        entity.ClearDomainEvents();

        foreach (var domainEvent in domainEvents)
            await mediator.Publish((object)domainEvent, cancellationToken);
    }
}
