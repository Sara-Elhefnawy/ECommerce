using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ECommerce.Domain.Abstractions.ImageCloudinary;
using Microsoft.Extensions.Options;

namespace ECommerce.Infrastructure.ImageCloudinary;

/// <summary>
/// PURPOSE: Abstracts Cloudinary API calls behind a clean interface
/// </summary>
public class CloudinaryService : ICloudinaryService
{
    // The Cloudinary client instance that makes the actual API calls
    // This is the main entry point for all Cloudinary operations
    private readonly Cloudinary _cloudinary;
    // Configuration settings (CloudName, ApiKey, ApiSecret)
    private readonly CloudinarySettings _settings;

    // initializes the Cloudinary client with credentials.
    // Called by Dependency Injection when ICloudinaryService is requested
    // The settings are injected via IOptions pattern, which reads from appsettings.json
    //   or environment variables (User Secrets, Azure App Settings, etc.)
    public CloudinaryService(IOptions<CloudinarySettings> settings)
    {
        _settings = settings.Value;
        var account = new Account(
            _settings.CloudName,
            _settings.ApiKey,
            _settings.ApiSecret
        );

        // Initialize the Cloudinary client with the account credentials
        // This client will be used for all subsequent API calls
        _cloudinary = new Cloudinary(account);

        // Force HTTPS for all Cloudinary URLs
        // This ensures images are served over secure connections
        // ALWAYS use Secure = true in production
        _cloudinary.Api.Secure = true;
    }

    public async Task<string> GetOptimizedImageUrlAsync(string publicId, CancellationToken ct = default)
    {
        // Build a URL with transformations
        // This doesn't call Cloudinary's API - it just builds the URL
        // The actual transformation happens when the URL is accessed
        var url = _cloudinary.Api.UrlImgUp
            .Transform(new Transformation()
                .Quality("auto")
                .FetchFormat("auto")
                .Width(500)
                .Crop("scale"))
            .BuildUrl(publicId);  // Build the final URL string

        // Return the URL as a Task (wrapped for async consistency)
        return await Task.FromResult(url);
    }

    public async Task<string> UploadImageAsync(Stream fileStream, string fileName, CancellationToken ct = default)
    {
        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(fileName, fileStream),
            UseFilename = true,
            UniqueFilename = false,
            Overwrite = true,
            Transformation = new Transformation()
                .Quality("auto")
                .FetchFormat("auto")
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null)
            throw new InvalidOperationException($"Cloudinary upload failed: {uploadResult.Error.Message}");


        // Return the secure URL that can be stored in the database
        // This URL is public and can be directly accessed by clients
        // Format: https://res.cloudinary.com/{cloud_name}/image/upload/v{version}/{public_id}.{format}
        return uploadResult.SecureUrl.ToString();
    }

    public async Task<string> UpdateImageAsync(Stream fileStream, string oldPublicId, string newFileName, CancellationToken ct = default)
    {
        await DeleteImageAsync(oldPublicId, ct);

        return await UploadImageAsync(fileStream, newFileName, ct);
    }

    public async Task<bool> DeleteImageAsync(string publicId, CancellationToken ct = default)
    {
        var deletionParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deletionParams);
        return result.Result is "ok";
    }

    /// <summary>
    /// Extracts the public ID from a Cloudinary URL.
    /// 
    /// WHY DO WE NEED THIS?
    /// - When we delete an image, we need its public ID
    /// - The database stores the full URL, not the public ID
    /// - This method extracts the public ID from the URL
    /// 
    /// URL FORMAT:
    /// https://res.cloudinary.com/{cloud_name}/image/upload/v{version}/{folder}/{public_id}.{extension}
    /// 
    /// EXAMPLE:
    /// URL: https://res.cloudinary.com/my-cloud/image/upload/v1234567890/products/ClassicWhiteTShirt.jpg
    /// Public ID: products/ClassicWhiteTShirt
    /// 
    /// NOTE: This is a HELPER method, not part of the interface.
    /// It's used internally by commands that need to delete old images.
    /// </summary>
    /// <param name="imageUrl">Full Cloudinary URL from the database</param>
    /// <returns>Public ID extracted from the URL, or empty string if not found</returns>
    public string ExtractPublicIdFromUrl(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl))
            return string.Empty;

        // Parse the URL into its components
        // This handles different URL formats safely
        var uri = new Uri(imageUrl);
        var segments = uri.Segments;       // Split URL by "/"

        // Find the "upload/" segment in the URL
        // This indicates where the Cloudinary part of the URL starts
        var uploadIndex = Array.IndexOf(segments, "upload/");
        if (uploadIndex is -1)
            return string.Empty;           // Not a valid Cloudinary URL

        // Get the last segment of the URL (the filename with extension)
        // Example: "ClassicWhiteTShirt.jpg"
        var publicIdWithExtension = segments[^1];

        // Remove the file extension to get just the public ID
        // Example: "ClassicWhiteTShirt.jpg" → "ClassicWhiteTShirt"
        var publicId = Path.GetFileNameWithoutExtension(publicIdWithExtension);

        // Note: If you have subfolders (e.g., "products/ClassicWhiteTShirt"),
        // you might need to combine segments differently.
        // This simple version handles the common case.
        return publicId;
    }
}
