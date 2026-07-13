using ECommerce.Domain.Common;
using ECommerce.Domain.Images;

namespace ECommerce.APP.Features.Products.Validators;

public static class ImageValidator
{
    public static Result Validate(long length,
        string fileName,
        string contentType)
    {
        if (length == 0)
            return Result.BadRequest(ProductErrors.ImageRequired);

        if (length > ImageRules.MaxImageSizeInBytes)
            return Result.BadRequest(ProductErrors.ImageTooLarge);

        var extension = Path.GetExtension(fileName)
            .ToLowerInvariant();

        if (!ImageRules.AllowedExtensions.Contains(extension))
            return Result.BadRequest(ProductErrors.InvalidImageExtension);

        if (!ImageRules.AllowedContentTypes.Contains(contentType))
            return Result.BadRequest(ProductErrors.InvalidImageType);

        return Result.Ok();
    }
}
