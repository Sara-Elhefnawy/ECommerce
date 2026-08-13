using ECommerce.Domain.Results;

namespace ECommerce.Domain.Abstractions.ImageCloudinary;

public interface ICloudinaryService
{
    Task<string> GetOptimizedImageUrlAsync(string publicId, CancellationToken ct = default);

    Task<ResultOfT<string>> UploadImageAsync(Stream fileStream, string fileName, CancellationToken ct = default);

    Task<ResultOfT<string>> UpdateImageAsync(Stream fileStream, string oldPublicId, string newFileName, CancellationToken ct = default);

    Task<ResultOfT<bool>> DeleteImageAsync(string publicId, CancellationToken ct = default);

    Task<bool> PingAsync(CancellationToken ct = default);
}
