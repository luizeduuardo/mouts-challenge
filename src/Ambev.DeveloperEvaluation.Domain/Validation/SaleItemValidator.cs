using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class SaleItemValidator : AbstractValidator<SaleItem>
{
    public SaleItemValidator()
    {
        RuleFor(s => s.ProductId)
            .NotEmpty().WithMessage("Product Id is required");

        RuleFor(s => s.ProductName)
            .NotEmpty().WithMessage("Product Name is required");

        RuleFor(i => i.UnitPrice)
            .GreaterThan(0).WithMessage("Unit price must be greater than 0");

        RuleFor(si => si.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0")
            .LessThanOrEqualTo(20).WithMessage("Cannot sell more than 20 identical items");
    }
}
