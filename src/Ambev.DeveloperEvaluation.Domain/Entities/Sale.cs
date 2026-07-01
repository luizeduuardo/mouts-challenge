using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Validation;
using FluentValidation;
using FluentValidation.Results;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Sale : BaseEntity
{
    public int SaleNumber { get; set; }

    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;

    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }
    public SaleStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<SaleItem> SaleItems { get; set; } = [];

    public Sale()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public ValidationResult Validate()
    {
        var validator = new SaleValidator();
        var result = validator.Validate(this);
        return result;
    }

    public void Cancel()
    {
        if (Status == SaleStatus.Cancelled)
            throw new DomainException("Sale is already cancelled");

        Status = SaleStatus.Cancelled;
        UpdateTime();
    }

    public void AddItem(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        if (Status == SaleStatus.Cancelled)
            throw new DomainException("Cannot add items to a cancelled sale");

        if (SaleItems.Any(si => si.ProductId == productId))
            throw new DomainException("Product already exists in sale. Update the quantity instead.");

        var item = new SaleItem(productId, productName, unitPrice, quantity);

        var validation = item.Validate();

        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        SaleItems.Add(item);
        RecalculateTotalAmount();
    }

    public void UpdateItemQuantity(Guid saleItemId, int newQuantity)
    {
        if (Status == SaleStatus.Cancelled)
            throw new DomainException("Cannot update items from a cancelled sale");

        var item = SaleItems.FirstOrDefault(si => si.Id == saleItemId)
            ?? throw new DomainException("Item not found");

        item.UpdateQuantity(newQuantity);
        RecalculateTotalAmount();
    }

    public void CancelItem(Guid saleItemId)
    {
        if (Status == SaleStatus.Cancelled)
            throw new DomainException("Cannot cancel items from a cancelled sale");

        var item = SaleItems.FirstOrDefault(si => si.Id == saleItemId)
            ?? throw new DomainException("Item not found");

        item.Cancel();
        RecalculateTotalAmount();
    }

    public void RemoveItem(Guid saleItemId)
    {
        if (Status == SaleStatus.Cancelled)
            throw new DomainException("Cannot remove items from a cancelled sale");

        var item = SaleItems.FirstOrDefault(si => si.Id == saleItemId)
            ?? throw new DomainException("Item not found");

        SaleItems.Remove(item);
        RecalculateTotalAmount();
    }

    private void RecalculateTotalAmount()
    {
        TotalAmount = SaleItems
            .Where(i => !i.IsCancelled)
            .Sum(i => i.TotalAmount);

        UpdateTime();
    }

    private void UpdateTime()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}