namespace ECommerce.Domain.Images;

public static class ImageRules
{
    public const long MaxImageSizeInBytes = 5 * 1024 * 1024;

    public static readonly string[] AllowedExtensions =
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".gif",
        ".webp"
    };

    public static readonly string[] AllowedContentTypes =
    {
        "image/jpeg",
        "image/png",
        "image/gif",
        "image/webp"
    };
}
