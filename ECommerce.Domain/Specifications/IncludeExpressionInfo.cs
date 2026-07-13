using System.Linq.Expressions;

namespace ECommerce.Domain.Specification;

public sealed record IncludeExpressionInfo(
        LambdaExpression LambdaExpression, // ThenInclude(p => p.sfs)
        LambdaExpression PreviousExpression // Include(p => p...)
    );
