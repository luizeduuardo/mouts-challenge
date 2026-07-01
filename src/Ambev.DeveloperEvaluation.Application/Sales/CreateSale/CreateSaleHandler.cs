using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public CreateSaleHandler(ISaleRepository saleRepository, IMapper mapper, IMediator mediator)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var sale = Sale.Create(
            command.CustomerId,
            command.CustomerName,
            command.BranchId,
            command.BranchName,
            command.SaleItems.Select(i => (i.ProductId, i.ProductName, i.Quantity, i.UnitPrice)));

        var validationResult = sale.Validate();

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);
        createdSale.MarkAsCreated();
        await _mediator.DispatchDomainEventsAsync(createdSale, cancellationToken);

        var result = _mapper.Map<CreateSaleResult>(createdSale);
        return result;
    }
}
