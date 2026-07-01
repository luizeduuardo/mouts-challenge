using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSaleItem;

public class UpdateSaleItemValidator : AbstractValidator<UpdateSaleItemCommand>
{
    public UpdateSaleItemValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty()
            .WithMessage("Sale ID is required");

        RuleFor(x => x.SaleItemId)
            .NotEmpty()
            .WithMessage("Sale item ID is required");

        RuleFor(x => x.NewQuantity)
            .GreaterThan(0)
            .LessThanOrEqualTo(20)
            .WithMessage("Cannot sell more than 20 identical items");
    }
}
