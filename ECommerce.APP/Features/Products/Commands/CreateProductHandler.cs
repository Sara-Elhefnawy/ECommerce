using ECommerce.APP.Features.Brands.Queries.GetByName;
using ECommerce.APP.Features.Products.Validators;
using ECommerce.APP.Features.Types.Queries.GetByName;
using ECommerce.APP.Mediator;
using ECommerce.Domain.Abstractions.ImageCloudinary;
using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Errors;
using ECommerce.Domain.Results;

namespace ECommerce.APP.Features.Products.Commands;

public sealed class CreateProductHandler(
    IUnitOfWork uow,
    IMediator mediator,
    ICloudinaryService cloudinaryService) :
    IRequestHandler<CreateProductCommand, ResultOfT<CreateProductResponse>>
{
    public async Task<ResultOfT<CreateProductResponse>> Handle(
        CreateProductCommand request,
        CancellationToken ct = default)
    {
        var imageValidation = ImageValidator.Validate(
            request.ImageLength,
            Path.GetExtension(request.ImageFileName),
            request.ImageContentType);

        if (imageValidation.IsFailure)
            return imageValidation.Error!;

        var brandExists = await mediator.Send(new GetBrandByNameQuery(request.BrandName), ct);
        if (brandExists.IsFailure)
            return ProductErrors.InvalidBrand;

        var typeExists = await mediator.Send(new GetTypeByNameQuery(request.TypeName), ct);
        if (typeExists.IsFailure)
            return ProductErrors.InvalidType;

        // Upload image to Cloudinary
        string imageUrl;
        using (var stream = request.ImageStream)
        {
            imageUrl = await cloudinaryService.UploadImageAsync(stream, request.ImageFileName, ct);
        }

        var result = Product.Create(
            request.Name, request.Description, imageUrl,
            request.Price, brandExists.Value.Id, typeExists.Value.Id);

        if (result.IsFailure)
            return result.Error!;

        var productRepo = uow.Repository<Product>();
        productRepo.Add(result.Value);
        await uow.SaveChangesAsync(ct);

        return ResultOfT<CreateProductResponse>.Created(
            new CreateProductResponse(
                result.Value.Id,
                result.Value.Name,
                result.Value.Description,
                result.Value.PictureUrl,
                result.Value.Price,
                brandExists.Value.Name,
                typeExists.Value.Name
            ));
    }
}
