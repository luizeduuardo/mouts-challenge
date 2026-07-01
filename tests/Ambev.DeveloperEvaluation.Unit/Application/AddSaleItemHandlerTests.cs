using Ambev.DeveloperEvaluation.Application.Sales.AddSaleItem;
using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class AddSaleItemHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly AddSaleItemHandler _handler;

    public AddSaleItemHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _mediator = Substitute.For<IMediator>();
        _handler = new AddSaleItemHandler(_saleRepository, _mapper, _mediator);
    }

    [Fact(DisplayName = "Given a valid item When adding to sale Then returns updated sale result")]
    public async Task Handle_ValidItem_ReturnsSuccessResponse()
    {
        // Given
        var sale = SaleTestData.GenerateValidSale();
        sale.Id = Guid.NewGuid();
        var command = new AddSaleItemCommand
        {
            SaleId = sale.Id,
            ProductId = AddSaleItemHandlerTestData.GenerateValidProductId(),
            ProductName = AddSaleItemHandlerTestData.GenerateValidProductName(),
            UnitPrice = AddSaleItemHandlerTestData.GenerateValidUnitPrice(),
            Quantity = AddSaleItemHandlerTestData.GenerateValidQuantity()
        };
        var result = new SaleResult { Id = sale.Id };

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(sale, Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<SaleResult>(sale).Returns(result);

        // When
        var addResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        addResult.Should().BeSameAs(result);
        sale.SaleItems.Should().ContainSingle(i => i.ProductId == command.ProductId);
        await _saleRepository.Received(1).UpdateAsync(sale, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given a nonexistent sale When adding an item Then throws KeyNotFoundException")]
    public async Task Handle_SaleNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var command = new AddSaleItemCommand
        {
            SaleId = Guid.NewGuid(),
            ProductId = AddSaleItemHandlerTestData.GenerateValidProductId(),
            ProductName = AddSaleItemHandlerTestData.GenerateValidProductName(),
            UnitPrice = AddSaleItemHandlerTestData.GenerateValidUnitPrice(),
            Quantity = AddSaleItemHandlerTestData.GenerateValidQuantity()
        };
        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>();
        await _saleRepository.DidNotReceive().UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given a quantity above the limit When adding an item Then throws ValidationException")]
    public async Task Handle_QuantityAboveLimit_ThrowsValidationException()
    {
        // Given
        var sale = SaleTestData.GenerateValidSale();
        sale.Id = Guid.NewGuid();
        var command = new AddSaleItemCommand
        {
            SaleId = sale.Id,
            ProductId = AddSaleItemHandlerTestData.GenerateValidProductId(),
            ProductName = AddSaleItemHandlerTestData.GenerateValidProductName(),
            UnitPrice = AddSaleItemHandlerTestData.GenerateValidUnitPrice(),
            Quantity = AddSaleItemHandlerTestData.GenerateQuantityAboveLimit()
        };
        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        await _saleRepository.DidNotReceive().UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given a valid item When adding to sale Then publishes SaleModifiedEvent")]
    public async Task Handle_ValidItem_PublishesSaleModifiedEvent()
    {
        // Given
        var sale = SaleTestData.GenerateValidSale();
        sale.Id = Guid.NewGuid();
        var command = new AddSaleItemCommand
        {
            SaleId = sale.Id,
            ProductId = AddSaleItemHandlerTestData.GenerateValidProductId(),
            ProductName = AddSaleItemHandlerTestData.GenerateValidProductName(),
            UnitPrice = AddSaleItemHandlerTestData.GenerateValidUnitPrice(),
            Quantity = AddSaleItemHandlerTestData.GenerateValidQuantity()
        };

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(sale, Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<SaleResult>(sale).Returns(new SaleResult { Id = sale.Id });

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _mediator.Received(1).Publish(Arg.Is<object>(o => o is SaleModifiedEvent), Arg.Any<CancellationToken>());
    }
}
