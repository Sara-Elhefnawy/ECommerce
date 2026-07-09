using System.Linq.Expressions;

namespace ECommerce.APP.Specifications;

public interface IOrderedSpecificationBuilder<T> : ISpecificationBuilder<T>
{
    IOrderedSpecificationBuilder<T> ThenBy(Expression<Func<T, object?>> orderExpression);
    IOrderedSpecificationBuilder<T> ThenByDescending(Expression<Func<T, object?>> orderExpression);
}

public interface IOrderedSpecificationBuilder<T, TResult>
    : ISpecificationBuilder<T, TResult>
{
    IOrderedSpecificationBuilder<T, TResult> ThenBy(Expression<Func<T, object?>> orderExpression);

    IOrderedSpecificationBuilder<T, TResult> ThenByDescending(Expression<Func<T, object?>> orderExpression);
}
