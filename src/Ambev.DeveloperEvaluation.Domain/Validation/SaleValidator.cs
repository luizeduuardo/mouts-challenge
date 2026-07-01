using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class SaleValidator : AbstractValidator<Sale>
{
    public SaleValidator()
    {
        RuleFor(s => s.CustomerId)
            .NotEmpty().WithMessage("Customer Id is required");

        RuleFor(s => s.CustomerName)
            .NotEmpty().WithMessage("Customer Name is required");

        RuleFor(s => s.BranchId)
            .NotEmpty().WithMessage("Branch Id is required");

        RuleFor(s => s.BranchName)
            .NotEmpty().WithMessage("Branch Name is required");

        RuleForEach(s => s.SaleItems)
            .SetValidator(new SaleItemValidator());
    }
}
