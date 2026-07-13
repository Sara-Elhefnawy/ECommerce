using System.Linq.Expressions;

namespace ECommerce.Domain.Specification;

public sealed record OrderExpressionInfo<T>(
    Expression<Func<T, object?>> KeySelector,
    OrderType OrderType);

public enum OrderType
{
    OrderBy,
    OrderByDescending,
    ThenBy,
    ThenByDescending,
}
