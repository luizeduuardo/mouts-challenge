using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Events;
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

    public static Sale Create(
        Guid customerId,
        string customerName,
        Guid branchId,
        string branchName,
        IEnumerable<(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice)> items)
    {
        var sale = new Sale
        {
            CustomerId = customerId,
            CustomerName = customerName,
            BranchId = branchId,
            BranchName = branchName
        };

        foreach (var item in items)
            sale.AddItem(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice);

        sale.ClearDomainEvents();

        return sale;
    }

    public void MarkAsCreated()
    {
        AddDomainEvent(new SaleCreatedEvent(Id, SaleNumber, CustomerId, CustomerName, BranchId, BranchName, TotalAmount, CreatedAt));
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

        AddDomainEvent(new SaleCancelledEvent(Id, SaleNumber, UpdatedAt));
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

        AddDomainEvent(new SaleModifiedEvent(Id, SaleNumber, TotalAmount, UpdatedAt));
    }

    public void UpdateItemQuantity(Guid saleItemId, int newQuantity)
    {
        if (Status == SaleStatus.Cancelled)
            throw new DomainException("Cannot update items from a cancelled sale");

        var item = SaleItems.FirstOrDefault(si => si.Id == saleItemId)
            ?? throw new DomainException("Item not found");

        item.UpdateQuantity(newQuantity);
        RecalculateTotalAmount();

        AddDomainEvent(new SaleModifiedEvent(Id, SaleNumber, TotalAmount, UpdatedAt));
    }

    public void CancelItem(Guid saleItemId)
    {
        if (Status == SaleStatus.Cancelled)
            throw new DomainException("Cannot cancel items from a cancelled sale");

        var item = SaleItems.FirstOrDefault(si => si.Id == saleItemId)
            ?? throw new DomainException("Item not found");

        item.Cancel();
        RecalculateTotalAmount();

        AddDomainEvent(new ItemCancelledEvent(Id, item.Id, item.ProductId, item.ProductName, UpdatedAt));
    }

    public void RemoveItem(Guid saleItemId)
    {
        if (Status == SaleStatus.Cancelled)
            throw new DomainException("Cannot remove items from a cancelled sale");

        var item = SaleItems.FirstOrDefault(si => si.Id == saleItemId)
            ?? throw new DomainException("Item not found");

        SaleItems.Remove(item);
        RecalculateTotalAmount();

        AddDomainEvent(new SaleModifiedEvent(Id, SaleNumber, TotalAmount, UpdatedAt));
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