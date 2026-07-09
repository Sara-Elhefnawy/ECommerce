using ECommerce.APP.Abstractions.Mediator;
using ECommerce.APP.Brands.Queries.Details;
using ECommerce.APP.Products.Extensions;
using ECommerce.APP.Types.Queries.Details;
using ECommerce.Domain.Abstractions.Cloudinaryy;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Products.Commands;

public sealed class CreateProductCommand(
    IUnitOfWork uow,
    IMediator mediator,
    ICloudinaryService cloudinaryService) :
    IRequestHandler<CreateProductRequest, ResultOfT<CreateProductResponse>>
{
    public async Task<ResultOfT<CreateProductResponse>> Handle(
        CreateProductRequest request,
        CancellationToken ct = default)
    {
        var imageValidation = Product.ValidateImageFile(request.ImageFile);
        if (imageValidation.IsFailure)
            return imageValidation.Error!;

        var brandExists = await mediator.Send(new DetailsBrandQuery(request.BrandId), ct);
        if (brandExists.IsFailure)
            return ProductErrors.InvalidBrand;

        var typeExists = await mediator.Send(new DetailsTypeQuery(request.TypeId), ct);
        if (typeExists.IsFailure)
            return ProductErrors.InvalidType;

        // Upload image to Cloudinary
        string imageUrl;
        using (var stream = request.ImageFile.OpenReadStream())
        {
            imageUrl = await cloudinaryService.UploadImageAsync(stream, request.ImageFile.FileName, ct);
        }

        var result = Product.Create(
            request.Name, request.Description, imageUrl,
            request.Price, request.BrandId, request.TypeId);

        if (result.IsFailure)
            return result.Error!;

        var productRepo = uow.Repository<Product>();
        productRepo.Add(result.Value);
        await uow.SaveChangesAsync(ct);

        return ResultOfT<CreateProductResponse>.Created(result.Value.ToCreateResponse());
    }
}
