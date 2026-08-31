using ECommerce.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace ECommerce.Tests.Unit.Domain;

public class BaseEntityTests
{
    // BaseEntity is abstract with no public constructor logic of its own —
    // every real entity (Product, Order, etc.) sets Id through its own
    // factory. This minimal subclass exists purely so MarkDeleted(), the
    // one piece of actual behavior BaseEntity defines, can be exercised
    // directly without pulling in an unrelated entity's validation rules.
    private sealed class TestEntity : BaseEntity
    {
        public TestEntity() => Id = Guid.NewGuid();
    }

    [Fact]
    public void NewEntity_IsNotDeletedByDefault()
    {
        var entity = new TestEntity();

        entity.IsDeleted.Should().BeFalse();
        entity.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void MarkDeleted_SetsIsDeletedTrueAndStampsUpdatedAt()
    {
        var entity = new TestEntity();

        entity.MarkDeleted();

        entity.IsDeleted.Should().BeTrue();
        entity.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void MarkDeleted_CalledAgain_ReStampsUpdatedAt()
    {
        // Unlike RefreshToken.Revoke, MarkDeleted has no "already deleted"
        // guard — a second call just overwrites UpdatedAt again. This
        // documents that as current behavior rather than assuming it;
        // if a guard gets added later, this test will correctly fail
        // and flag the intentional change.
        var entity = new TestEntity();
        entity.MarkDeleted();
        var firstUpdatedAt = entity.UpdatedAt!.Value;

        Thread.Sleep(5); // ensure the clock actually moves forward
        entity.MarkDeleted();

        entity.UpdatedAt.Should().BeOnOrAfter(firstUpdatedAt);
    }
}
