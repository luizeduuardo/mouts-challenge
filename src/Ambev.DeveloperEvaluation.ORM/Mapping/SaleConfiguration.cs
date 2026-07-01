using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(s => s.SaleNumber).ValueGeneratedOnAdd();
        builder.Property(s => s.CustomerId).HasColumnType("uuid");
        builder.Property(s => s.BranchId).HasColumnType("uuid");

        builder.Property(s => s.Status)
            .HasConversion<string>();

        builder.HasMany(s => s.SaleItems)
           .WithOne(si => si.Sale)
           .HasForeignKey(si => si.SaleId)
           .OnDelete(DeleteBehavior.Cascade);
    }
}
