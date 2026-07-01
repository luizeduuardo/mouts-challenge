using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSaleItem;

public class UpdateSaleItemHandler : IRequestHandler<UpdateSaleItemCommand, SaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public UpdateSaleItemHandler(ISaleRepository saleRepository, IMapper mapper, IMediator mediator)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<SaleResult> Handle(UpdateSaleItemCommand request, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleItemValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken)
            ?? throw new KeyNotFoundException($"Sale with ID {request.SaleId} not found");

        sale.UpdateItemQuantity(request.SaleItemId, request.NewQuantity);

        var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);
        await _mediator.DispatchDomainEventsAsync(updatedSale, cancellationToken);

        return _mapper.Map<SaleResult>(updatedSale);
    }
}
