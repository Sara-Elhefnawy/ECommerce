using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistent.Configuration;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(x => x.Status)
            .HasConversion<int>();

        builder.Property(x => x.DeliveryMethodName)
            .HasMaxLength(Order.MaxDeliveryMethodNameLength);

        builder.Property(x => x.DeliveryMethodPrice)
            .HasPrecision(18, 2);

        builder.Property(x => x.DeliveryMethodEstimatedTime)
            .HasMaxLength(Order.MaxDeliveryTimeLength);

        builder.OwnsOne(x => x.ShippingAddress, address =>
        {
            address.Property(a => a.RecipientFirstName)
                .HasColumnName("ShippingRecipientFirstName")
                .HasMaxLength(ShippingAddress.MaxNameLength);

            address.Property(a => a.RecipientLastName)
                .HasColumnName("ShippingRecipientLastName")
                .HasMaxLength(ShippingAddress.MaxNameLength);

            address.Property(a => a.PhoneNumber)
                .HasColumnName("ShippingPhoneNumber")
                .HasMaxLength(ShippingAddress.MaxPhoneLength);

            address.Property(a => a.Country)
                .HasColumnName("ShippingCountry")
                .HasMaxLength(ShippingAddress.MaxCountryLength);

            address.Property(a => a.City)
                .HasColumnName("ShippingCity")
                .HasMaxLength(ShippingAddress.MaxCityLength);

            address.Property(a => a.Street)
                .HasColumnName("ShippingStreet")
                .HasMaxLength(ShippingAddress.MaxStreetLength);

            address.Property(a => a.PostalCode)
                .HasColumnName("ShippingPostalCode")
                .HasMaxLength(ShippingAddress.MaxPostalCodeLength);
        });

        builder.Property(x => x.ItemsTotal)
            .HasPrecision(18, 2);

        builder.Property(x => x.ShippingCost)
            .HasPrecision(18, 2);

        builder.Property(x => x.Total)
            .HasPrecision(18, 2);

        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Items is a computed property EF's default behavior tries to use the property
        // to materialize data — but there's no setter, so EF can't assign a list to it.
        // Without telling EF to use the _items field instead, EF will either throw on
        // model build or silently fail to populate Items when querying
        builder.Navigation(x => x.Items)
            .HasField("_items")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.CreatedAt);

        builder.HasQueryFilter(h => !h.IsDeleted);
    }
}
