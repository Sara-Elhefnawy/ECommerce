using ECommerce.APP.Cachings;
using ECommerce.APP.Features.Carts.Commands.AddItemToCart.ProductLookup;
using ECommerce.APP.Features.Carts.Queries.GetCart;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Common;
using ECommerce.Domain.Common.Errors;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.Carts.Commands.AddItemToCart;

public sealed class AddCartItemHandler(
    ICartRepository repo,
    IReadRepository<Product> productRepository)
    : IRequestHandler<AddCartItemCommand, ResultOfT<GetCartResponse>>
{
    public async Task<ResultOfT<GetCartResponse>> Handle(
        AddCartItemCommand request,
        CancellationToken cancellationToken)
    {
        var product = await productRepository.FirstOrDefaultAsync(
            new ProductForCartSpecification(request.ProductId),
            cancellationToken);

        if (product is null)
            return ResultOfT<GetCartResponse>.Failure(ProductErrors.NotFound);

        var cart = await repo.GetOrCreateAsync(request.BuyerId, cancellationToken);

        var addResult = cart.AddItem(
            product.Id, product.Name, product.PictureUrl, product.Price, request.Quantity);

        if (addResult.IsFailure)
            return ResultOfT<GetCartResponse>.Failure(addResult.Error!);

        await repo.SaveAsync(cart, cancellationToken);

        return ResultOfT<GetCartResponse>.Ok(GetCartMapper.ToResponse(cart));
    }
}
