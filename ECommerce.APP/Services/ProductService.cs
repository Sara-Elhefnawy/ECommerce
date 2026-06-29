using ECommerce.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace ECommerce.APP.Services;

public class ProductService
{
    private readonly ILogger<ProductService> _logger;

    public ProductService(ILogger<ProductService> logger)
    {
        _logger = logger;
    }

    public async Task<Product> CreateProductAsync(string name, decimal price)
    {
        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["Operation"] = "ProductCreation"
        }))
        {
            _logger.LogInformation("Starting product creation for: {ProductName}", name);

            var result = Product.Create(name, "Description", "url", price, Guid.NewGuid(), Guid.NewGuid());

            if (result.IsSuccess)
            {
                using (_logger.BeginScope(new Dictionary<string, object>
                {
                    ["CreatedProductId"] = result.Value.Id
                }))
                {
                    _logger.LogInformation("✅ Product created successfully: {ProductName} (ID: {CreatedProductId})",
                        name, result.Value.Id);
                }
                return result.Value;
            }
            else
            {
                _logger.LogWarning("❌ Product creation failed: {ErrorCode} - {ErrorMessage}",
                    result.Error?.Code, result.Error?.Message);
                throw new Exception("Product creation failed");
            }
        }
    }
}
