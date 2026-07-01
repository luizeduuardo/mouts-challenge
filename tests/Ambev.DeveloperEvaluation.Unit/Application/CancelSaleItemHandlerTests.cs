using Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;
using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class CancelSaleItemHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly CancelSaleItemHandler _handler;

    public CancelSaleItemHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _mediator = Substitute.For<IMediator>();
        _handler = new CancelSaleItemHandler(_saleRepository, _mapper, _mediator);
    }

    [Fact(DisplayName = "Given an existing item When cancelling it Then returns updated sale result")]
    public async Task Handle_ValidItem_ReturnsSuccessResponse()
    {
        // Given
        var sale = SaleTestData.GenerateValidSaleWithItems();
        sale.Id = Guid.NewGuid();
        var item = sale.SaleItems.First();
        item.Id = Guid.NewGuid();
        var command = new CancelSaleItemCommand { SaleId = sale.Id, SaleItemId = item.Id };
        var result = new SaleResult { Id = sale.Id };

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(sale, Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<SaleResult>(sale).Returns(result);

        // When
        var cancelResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        cancelResult.Should().BeSameAs(result);
        item.IsCancelled.Should().BeTrue();
        await _saleRepository.Received(1).UpdateAsync(sale, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given a nonexistent sale When cancelling an item Then throws KeyNotFoundException")]
    public async Task Handle_SaleNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var command = new CancelSaleItemCommand { SaleId = Guid.NewGuid(), SaleItemId = Guid.NewGuid() };
        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>();
        await _saleRepository.DidNotReceive().UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given a nonexistent item When cancelling it Then throws DomainException")]
    public async Task Handle_ItemNotFound_ThrowsDomainException()
    {
        // Given
        var sale = SaleTestData.GenerateValidSaleWithItems();
        sale.Id = Guid.NewGuid();
        var command = new CancelSaleItemCommand { SaleId = sale.Id, SaleItemId = Guid.NewGuid() };
        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<DomainException>();
        await _saleRepository.DidNotReceive().UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given an existing item When cancelling it Then publishes ItemCancelledEvent")]
    public async Task Handle_ValidItem_PublishesItemCancelledEvent()
    {
        // Given
        var sale = SaleTestData.GenerateValidSaleWithItems();
        sale.Id = Guid.NewGuid();
        var item = sale.SaleItems.First();
        item.Id = Guid.NewGuid();
        var command = new CancelSaleItemCommand { SaleId = sale.Id, SaleItemId = item.Id };

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(sale, Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<SaleResult>(sale).Returns(new SaleResult { Id = sale.Id });

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _mediator.Received(1).Publish(Arg.Is<object>(o => o is ItemCancelledEvent), Arg.Any<CancellationToken>());
    }
}
