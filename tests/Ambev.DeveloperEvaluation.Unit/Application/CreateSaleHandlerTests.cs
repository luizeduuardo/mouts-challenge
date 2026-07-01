using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the <see cref="CreateSaleHandler"/> class.
/// </summary>
public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _mediator = Substitute.For<IMediator>();
        _handler = new CreateSaleHandler(_saleRepository, _mapper, _mediator);

        // CreateAsync simulates the database populating Id/SaleNumber on the same instance
        // it's given, and returning that same instance - matching SaleRepository.CreateAsync.
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var sale = callInfo.Arg<Sale>();
                sale.Id = Guid.NewGuid();
                sale.SaleNumber = 4242;
                return Task.FromResult(sale);
            });
    }

    [Fact(DisplayName = "Given valid sale data When creating sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var result = new CreateSaleResult();
        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>()).Returns(result);

        // When
        var createSaleResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        createSaleResult.Should().BeSameAs(result);
        await _saleRepository.Received(1).CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given invalid sale data When creating sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new CreateSaleCommand(); // Empty command builds an invalid Sale (no CustomerId/BranchId)

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        await _saleRepository.DidNotReceive().CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given sale item quantity above the limit When creating sale Then throws validation exception")]
    public async Task Handle_ItemQuantityAboveLimit_ThrowsValidationException()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        command.SaleItems.Add(new SaleItemCommand
        {
            ProductId = Guid.NewGuid(),
            ProductName = "Extra Product",
            UnitPrice = 10m,
            Quantity = 25 // exceeds the max of 20
        });

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        await _saleRepository.DidNotReceive().CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given valid command When handling Then builds a sale with the command's items")]
    public async Task Handle_ValidRequest_BuildsSaleWithCommandItems()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>()).Returns(new CreateSaleResult());

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).CreateAsync(
            Arg.Is<Sale>(s =>
                s.CustomerId == command.CustomerId &&
                s.CustomerName == command.CustomerName &&
                s.BranchId == command.BranchId &&
                s.BranchName == command.BranchName &&
                s.SaleItems.Count == command.SaleItems.Count),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given valid command When handling Then maps created sale to result")]
    public async Task Handle_ValidRequest_MapsCreatedSaleToResult()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var result = new CreateSaleResult();
        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>()).Returns(result);

        // When
        var createSaleResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        _mapper.Received(1).Map<CreateSaleResult>(Arg.Is<Sale>(s => s.Id != Guid.Empty));
        createSaleResult.Should().BeSameAs(result);
    }

    [Fact(DisplayName = "Given valid command When handling Then publishes SaleCreatedEvent with the persisted sale's id")]
    public async Task Handle_ValidRequest_PublishesSaleCreatedEvent()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>()).Returns(new CreateSaleResult());

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _mediator.Received(1).Publish(
            Arg.Is<object>(o => o is SaleCreatedEvent && ((SaleCreatedEvent)o).SaleNumber == 4242),
            Arg.Any<CancellationToken>());
    }
}
