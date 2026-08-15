using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistent.Configuration;

public sealed class DeliveryMethodConfiguration : IEntityTypeConfiguration<DeliveryMethod>
{
    public void Configure(EntityTypeBuilder<DeliveryMethod> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(DeliveryMethod.MaxNameLength);

        builder.Property(x => x.Description)
            .HasMaxLength(DeliveryMethod.MaxDescriptionLength);

        builder.Property(x => x.Price)
            .HasPrecision(18, 2);

        builder.Property(x => x.EstimatedDeliveryTime)
            .HasMaxLength(DeliveryMethod.MaxDeliveryTimeLength);

        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => x.DisplayOrder);

        builder.HasQueryFilter(h => !h.IsDeleted);
    }
}
