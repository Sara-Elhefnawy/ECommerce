using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistent.Configuration;

public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.OwnsOne(x => x.ItemOrdered, snapshot =>
        {
            snapshot.Property(p => p.ProductId)
                .HasColumnName("ProductId");

            snapshot.Property(p => p.ProductName)
                .HasColumnName("ProductName")
                .HasMaxLength(ProductItemOrdered.MaxProductNameLength);

            snapshot.Property(p => p.PictureUrl)
                .HasColumnName("PictureUrl")
                .HasMaxLength(ProductItemOrdered.MaxPictureUrlLength);

            snapshot.Property(p => p.UnitPrice)
                .HasColumnName("UnitPrice")
                .HasPrecision(18, 2);

            snapshot.HasIndex(p => p.ProductId);
        });

        builder.HasIndex(x => x.OrderId);

        builder.Ignore(x => x.SubTotalPrice);
    }
}
