using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
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
    private readonly CreateSaleHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSaleHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new CreateSaleHandler(_saleRepository, _mapper);
    }

    /// <summary>
    /// Builds the Sale entity that AutoMapper would produce for the given command,
    /// mirroring the mapping configured in <see cref="CreateSaleProfile"/>.
    /// </summary>
    private static Sale MapCommandToSale(CreateSaleCommand command)
    {
        var sale = new Sale
        {
            CustomerId = command.CustomerId,
            CustomerName = command.CustomerName,
            BranchId = command.BranchId,
            BranchName = command.BranchName,
            Status = SaleStatus.Active
        };

        foreach (var item in command.SaleItems)
            sale.SaleItems.Add(new SaleItem(item.ProductId, item.ProductName, item.UnitPrice, item.Quantity));

        return sale;
    }

    /// <summary>
    /// Tests that a valid sale creation request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid sale data When creating sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var sale = MapCommandToSale(command);
        var createdSale = MapCommandToSale(command);
        createdSale.Id = Guid.NewGuid();

        var result = new CreateSaleResult { Id = createdSale.Id };

        _mapper.Map<Sale>(command).Returns(sale);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(createdSale);
        _mapper.Map<CreateSaleResult>(createdSale).Returns(result);

        // When
        var createSaleResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        createSaleResult.Should().NotBeNull();
        createSaleResult.Id.Should().Be(createdSale.Id);
        await _saleRepository.Received(1).CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a sale with missing customer/branch data throws a validation exception
    /// and is never persisted.
    /// </summary>
    [Fact(DisplayName = "Given invalid sale data When creating sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new CreateSaleCommand(); // Empty command maps to an invalid Sale
        var sale = MapCommandToSale(command);

        _mapper.Map<Sale>(command).Returns(sale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        await _saleRepository.DidNotReceive().CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a sale item exceeding the maximum allowed quantity fails validation,
    /// exercising the SaleItemValidator rule enforced through Sale.Validate().
    /// </summary>
    [Fact(DisplayName = "Given sale item quantity above the limit When creating sale Then throws validation exception")]
    public async Task Handle_ItemQuantityAboveLimit_ThrowsValidationException()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var sale = MapCommandToSale(command);
        sale.SaleItems.Add(new SaleItem(Guid.NewGuid(), "Extra Product", 10m, 25)); // exceeds the max of 20

        _mapper.Map<Sale>(command).Returns(sale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        await _saleRepository.DidNotReceive().CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the mapper is called with the correct command.
    /// </summary>
    [Fact(DisplayName = "Given valid command When handling Then maps command to sale entity")]
    public async Task Handle_ValidRequest_MapsCommandToSale()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var sale = MapCommandToSale(command);

        _mapper.Map<Sale>(command).Returns(sale);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        _mapper.Received(1).Map<Sale>(Arg.Is<CreateSaleCommand>(c =>
            c.CustomerId == command.CustomerId &&
            c.CustomerName == command.CustomerName &&
            c.BranchId == command.BranchId &&
            c.BranchName == command.BranchName &&
            c.SaleItems.Count == command.SaleItems.Count));
    }

    /// <summary>
    /// Tests that the sale returned by the repository is passed to the mapper to build the result.
    /// </summary>
    [Fact(DisplayName = "Given valid command When handling Then maps created sale to result")]
    public async Task Handle_ValidRequest_MapsCreatedSaleToResult()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var sale = MapCommandToSale(command);
        var createdSale = MapCommandToSale(command);
        createdSale.Id = Guid.NewGuid();
        var result = new CreateSaleResult { Id = createdSale.Id };

        _mapper.Map<Sale>(command).Returns(sale);
        _saleRepository.CreateAsync(sale, Arg.Any<CancellationToken>()).Returns(createdSale);
        _mapper.Map<CreateSaleResult>(createdSale).Returns(result);

        // When
        var createSaleResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        _mapper.Received(1).Map<CreateSaleResult>(createdSale);
        createSaleResult.Should().BeSameAs(result);
    }
}
