using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.RemoveSaleItem;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class RemoveSaleItemHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly RemoveSaleItemHandler _handler;

    public RemoveSaleItemHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new RemoveSaleItemHandler(_saleRepository, _mapper);
    }

    [Fact(DisplayName = "Given an existing item When removing it Then returns updated sale result")]
    public async Task Handle_ValidItem_ReturnsSuccessResponse()
    {
        // Given
        var sale = SaleTestData.GenerateValidSaleWithItems();
        sale.Id = Guid.NewGuid();
        var item = sale.SaleItems.First();
        item.Id = Guid.NewGuid();
        var command = new RemoveSaleItemCommand { SaleId = sale.Id, SaleItemId = item.Id };
        var result = new SaleResult { Id = sale.Id };

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(sale, Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<SaleResult>(sale).Returns(result);

        // When
        var removeResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        removeResult.Should().BeSameAs(result);
        sale.SaleItems.Should().NotContain(i => i.Id == item.Id);
        await _saleRepository.Received(1).UpdateAsync(sale, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given a nonexistent sale When removing an item Then throws KeyNotFoundException")]
    public async Task Handle_SaleNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var command = new RemoveSaleItemCommand { SaleId = Guid.NewGuid(), SaleItemId = Guid.NewGuid() };
        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>();
        await _saleRepository.DidNotReceive().UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given a nonexistent item When removing it Then throws DomainException")]
    public async Task Handle_ItemNotFound_ThrowsDomainException()
    {
        // Given
        var sale = SaleTestData.GenerateValidSaleWithItems();
        sale.Id = Guid.NewGuid();
        var command = new RemoveSaleItemCommand { SaleId = sale.Id, SaleItemId = Guid.NewGuid() };
        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<DomainException>();
        await _saleRepository.DidNotReceive().UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }
}
