using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Validation;
using FluentValidation.Results;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SaleItem : BaseEntity
{
    public Guid SaleId { get; set; }
    public virtual Sale Sale { get; set; } = default!;

    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal Discount { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsCancelled { get; set; }

    public SaleItem(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;

        ApplyDiscount();
    }

    public ValidationResult Validate()
    {
        var validator = new SaleItemValidator();
        var result = validator.Validate(this);
        return result;
    }

    public void Cancel()
    {
        IsCancelled = true;
    }

    public void UpdateQuantity(int newQuantity)
    {
        Quantity = newQuantity;

        ApplyDiscount();
    }

    private void ApplyDiscount()
    {
        Discount = Quantity switch
        {
            >= 10 and <= 20 => 0.20m,
            >= 4 => 0.10m,
            _ => 0m
        };

        TotalAmount = UnitPrice * Quantity * (1 - Discount);
    }
}