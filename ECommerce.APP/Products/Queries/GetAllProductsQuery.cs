using ECommerce.APP.Products.Responses;
using ECommerce.Domain.Common;

namespace ECommerce.APP.Products.Queries;

public sealed class GetAllProductsQuery(IProductQueryService queryService)
{
    public async Task<ResultOfT<IReadOnlyList<GetAllProductsResponse>>> Execute(CancellationToken ct = default)
    {
        var products = await queryService.GetAllAsync(ct);

        if (products is null || !products.Any())
            return ProductErrors.NotFound;

        return ResultOfT<IReadOnlyList<GetAllProductsResponse>>.Ok(products);
    }
    /*
         Why "return products;" does NOT work here (but works for single objects)?

         C# has a hard rule: a user-defined implicit/explicit conversion operator
         can NEVER have an interface as its source or target type. 
            This isn't a quirk of generics or EF Core — it's baked into the C# language spec.

         Our operator is:
             public static implicit operator ResultOfT<T>(T value) => Ok(value);

         When T is a class (e.g. Product, GetAllProductsResponse), this operator
         is legal and the compiler happily uses it — 
            that's why "return product;" works fine in single-item queries like GetById.

         But here, T = IReadOnlyList<GetAllProductsResponse> — an INTERFACE.
         The moment T gets substituted with an interface, that specific version
         of the operator becomes illegal, and the compiler refuses to use it (CS0029). 
         No LangVersion setting or workaround changes this — it's a
         permanent restriction, not a temporary limitation.

         Why the ban exists: interfaces already have their own built-in conversion
         rules (any List<T> is automatically usable as IReadOnlyList<T> with zero
         extra code). Letting user-defined operators also target interfaces would
         create ambiguity, since one class can implement many interfaces at once.
         So the language spec simply disallows it outright.

         The takeaway: any query returning an interface-typed collection
         (IReadOnlyList<T>, IEnumerable<T>, ICollection<T>, etc.) must always
         construct the Result explicitly with .Ok(...) — implicit conversion is
         not an option here, by design, not by mistake.
    */
}
