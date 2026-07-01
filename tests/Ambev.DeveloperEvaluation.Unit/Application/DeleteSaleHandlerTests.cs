using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class DeleteSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly DeleteSaleHandler _handler;

    public DeleteSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _handler = new DeleteSaleHandler(_saleRepository);
    }

    [Fact(DisplayName = "Given an existing sale When deleting Then removes it via repository")]
    public async Task Handle_ExistingSale_DeletesSale()
    {
        // Given
        var sale = SaleTestData.GenerateValidSaleWithItems();
        sale.Id = Guid.NewGuid();
        var command = new DeleteSaleCommand { SaleId = sale.Id };

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).DeleteAsync(sale, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given a nonexistent sale When deleting Then throws KeyNotFoundException")]
    public async Task Handle_SaleNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var command = new DeleteSaleCommand { SaleId = Guid.NewGuid() };
        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>();
        await _saleRepository.DidNotReceive().DeleteAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }
}
