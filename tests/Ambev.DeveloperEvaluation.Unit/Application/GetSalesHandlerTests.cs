using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.GetSales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class GetSalesHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetSalesHandler _handler;

    public GetSalesHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetSalesHandler(_saleRepository, _mapper);
    }

    [Fact(DisplayName = "Given a page of sales When listing Then returns mapped paged result")]
    public async Task Handle_ValidQuery_ReturnsPagedResult()
    {
        // Given
        var query = new GetSalesQuery { PageNumber = 2, PageSize = 5 };
        var sale1 = SaleTestData.GenerateValidSale();
        sale1.Id = Guid.NewGuid();
        var sale2 = SaleTestData.GenerateValidSale();
        sale2.Id = Guid.NewGuid();
        var sales = new List<Sale> { sale1, sale2 };
        var mappedResults = new List<SaleResult> { new() { Id = sale1.Id }, new() { Id = sale2.Id } };

        _saleRepository.GetPagedAsync(
            Arg.Is<SaleFilter>(f => f.CustomerId == query.CustomerId && f.BranchId == query.BranchId && f.Status == query.Status),
            query.PageNumber,
            query.PageSize,
            Arg.Any<CancellationToken>())
            .Returns((sales, 12));

        _mapper.Map<List<SaleResult>>(sales).Returns(mappedResults);

        // When
        var result = await _handler.Handle(query, CancellationToken.None);

        // Then
        result.Items.Should().BeSameAs(mappedResults);
        result.TotalCount.Should().Be(12);
        result.PageNumber.Should().Be(2);
        result.PageSize.Should().Be(5);
    }

    [Fact(DisplayName = "Given a page size above the limit When listing Then throws ValidationException")]
    public async Task Handle_PageSizeAboveLimit_ThrowsValidationException()
    {
        // Given
        var query = new GetSalesQuery { PageNumber = 1, PageSize = 500 };

        // When
        var act = () => _handler.Handle(query, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}
