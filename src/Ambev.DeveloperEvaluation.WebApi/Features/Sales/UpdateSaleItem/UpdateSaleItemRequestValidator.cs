using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSaleItem;

public class UpdateSaleItemRequestValidator : AbstractValidator<UpdateSaleItemRequest>
{
    public UpdateSaleItemRequestValidator()
    {
        RuleFor(x => x.NewQuantity)
            .GreaterThan(0)
            .LessThanOrEqualTo(20)
            .WithMessage("Cannot sell more than 20 identical items");
    }
}
