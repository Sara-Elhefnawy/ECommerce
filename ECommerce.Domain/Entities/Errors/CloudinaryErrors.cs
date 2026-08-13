using ECommerce.Domain.Results;

namespace ECommerce.Domain.Entities.Errors;

public static class CloudinaryErrors
{
    public static readonly Error UploadFailed = 
        Error.Unavailable(
            "Cloudinary.UploadFailed", 
            "Failed to upload image. Please try again later.");

    public static readonly Error DeleteFailed = 
        Error.Unavailable(
            "Cloudinary.DeleteFailed", 
            "Failed to delete image. Please try again later.");
}
