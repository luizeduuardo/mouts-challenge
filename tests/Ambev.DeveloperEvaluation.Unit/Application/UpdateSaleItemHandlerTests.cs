using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSaleItem;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class UpdateSaleItemHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly UpdateSaleItemHandler _handler;

    public UpdateSaleItemHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new UpdateSaleItemHandler(_saleRepository, _mapper);
    }

    [Fact(DisplayName = "Given an existing item When updating quantity Then returns updated sale result")]
    public async Task Handle_ValidItem_ReturnsSuccessResponse()
    {
        // Given
        var sale = SaleTestData.GenerateValidSaleWithItems();
        sale.Id = Guid.NewGuid();
        var item = sale.SaleItems.First();
        item.Id = Guid.NewGuid();
        var command = new UpdateSaleItemCommand { SaleId = sale.Id, SaleItemId = item.Id, NewQuantity = 5 };
        var result = new SaleResult { Id = sale.Id };

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(sale, Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<SaleResult>(sale).Returns(result);

        // When
        var updateResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        updateResult.Should().BeSameAs(result);
        item.Quantity.Should().Be(5);
        await _saleRepository.Received(1).UpdateAsync(sale, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given a nonexistent sale When updating an item Then throws KeyNotFoundException")]
    public async Task Handle_SaleNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var command = new UpdateSaleItemCommand { SaleId = Guid.NewGuid(), SaleItemId = Guid.NewGuid(), NewQuantity = 5 };
        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>();
        await _saleRepository.DidNotReceive().UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given a nonexistent item When updating quantity Then throws DomainException")]
    public async Task Handle_ItemNotFound_ThrowsDomainException()
    {
        // Given
        var sale = SaleTestData.GenerateValidSaleWithItems();
        sale.Id = Guid.NewGuid();
        var command = new UpdateSaleItemCommand { SaleId = sale.Id, SaleItemId = Guid.NewGuid(), NewQuantity = 5 };
        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<DomainException>();
        await _saleRepository.DidNotReceive().UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }
}
