using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Mappings;

public class OrderMapping : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Status).IsRequired();
        builder.Property(o => o.Subtotal).HasColumnType("decimal(10,2)").IsRequired();
        builder.Property(o => o.Discount).HasColumnType("decimal(10,2)").IsRequired();
        builder.Property(o => o.Total).HasColumnType("decimal(10,2)").IsRequired();

        builder.HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
