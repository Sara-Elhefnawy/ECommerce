namespace ECommerce.Domain.Abstractions.Cloudinaryy;

public interface ICloudinaryService
{
    Task<string> GetOptimizedImageUrlAsync(string publicId, CancellationToken ct = default);

    Task<string> UploadImageAsync(Stream fileStream, string fileName, CancellationToken ct = default);

    Task<string> UpdateImageAsync(Stream fileStream, string oldPublicId, string newFileName, CancellationToken ct = default);

    Task<bool> DeleteImageAsync(string publicId, CancellationToken ct = default);
}
