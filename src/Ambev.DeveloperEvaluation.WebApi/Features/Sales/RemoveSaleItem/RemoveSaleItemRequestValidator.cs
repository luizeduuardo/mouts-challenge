using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.RemoveSaleItem;

public class RemoveSaleItemRequestValidator : AbstractValidator<RemoveSaleItemRequest>
{
    public RemoveSaleItemRequestValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty()
            .WithMessage("Sale ID is required");

        RuleFor(x => x.SaleItemId)
            .NotEmpty()
            .WithMessage("Sale item ID is required");
    }
}
