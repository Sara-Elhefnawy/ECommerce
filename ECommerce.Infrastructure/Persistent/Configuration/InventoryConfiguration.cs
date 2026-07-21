using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistent.Configuration;

public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.HasIndex(i => i.ProductId)
            .IsUnique();

        builder.HasOne(i => i.Product)
           .WithOne()
           .HasForeignKey<Inventory>(i => i.ProductId);

        builder.HasQueryFilter(h => !h.IsDeleted);
    }
}
