using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

public class CreateSaleRequestValidator : AbstractValidator<CreateSaleRequest>
{
    public CreateSaleRequestValidator()
    {
        RuleFor(sale => sale.CustomerId)
            .NotEmpty().WithMessage("Customer Id is required");

        RuleFor(sale => sale.CustomerName)
            .NotEmpty().WithMessage("Customer Name is required");

        RuleFor(sale => sale.BranchId)
            .NotEmpty().WithMessage("Branch Id is required");

        RuleFor(sale => sale.BranchName)
            .NotEmpty().WithMessage("Branch Name is required");

        RuleFor(sale => sale.SaleItems)
            .NotEmpty().WithMessage("Sale must contain at least one item");

        RuleForEach(sale => sale.SaleItems)
            .SetValidator(new SaleItemRequestValidator());
    }
}

public class SaleItemRequestValidator : AbstractValidator<SaleItemRequest>
{
    public SaleItemRequestValidator()
    {
        RuleFor(item => item.ProductId)
            .NotEmpty().WithMessage("Product Id is required");

        RuleFor(item => item.ProductName)
            .NotEmpty().WithMessage("Product Name is required");

        RuleFor(item => item.UnitPrice)
            .GreaterThan(0).WithMessage("Unit price must be greater than 0");

        RuleFor(item => item.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0")
            .LessThanOrEqualTo(20).WithMessage("Cannot sell more than 20 identical items");
    }
}
