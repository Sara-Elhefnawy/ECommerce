using ECommerce.Domain.Abstractions.Repositories;
using ECommerce.Domain.Common.Enums;
using ECommerce.Domain.Entities;
using Serilog.Context;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();

        // GET: /api/products - Get from DATABASE
        app.MapGet("/api/products", async ([FromServices] IUnitOfWork unitOfWork) =>
        {
            var productRepo = unitOfWork.Repository<Product>();
            var products = await productRepo.GetAllAsync();
            var activeProducts = products?.Where(p => !p.IsDeleted).ToList() ?? [];

            logger.LogInformation("Retrieved {Count} products from database", activeProducts.Count);

            return Results.Ok(activeProducts);
        })
        .WithName("GetProducts");

        // GET: /api/products/{id} - Get from DATABASE
        app.MapGet("/api/products/{id:guid}", async (
            Guid id, 
            [FromServices] IUnitOfWork unitOfWork) =>
        {
            using (LogContext.PushProperty("ProductId", id))
            {
                logger.LogInformation("Looking for product: {ProductId}", id);

                var productRepo = unitOfWork.Repository<Product>();
                var product = await productRepo.GetByIdAsync(id);

                if (product is null || product.IsDeleted)
                {
                    logger.LogWarning("Product not found: {ProductId}", id);
                    return Results.NotFound(new { message = $"Product with ID {id} not found" });
                }

                logger.LogInformation("Retrieved product: {ProductName} (ID: {ProductId})",
                    product.Name, product.Id);

                return Results.Ok(product);
            }
        })
        .WithName("GetProduct");

        // POST: /api/products - Save to DATABASE
        app.MapPost("/api/products", async (
            [FromBody] CreateProductRequest request, 
            [FromServices] IUnitOfWork unitOfWork) =>
        {
            using (LogContext.PushProperty("ProductName", request.Name))
            {
                logger.LogInformation("Creating product: {ProductName}", request.Name);

                var result = Product.Create(
                    request.Name,
                    request.Description,
                    request.PictureUrl,
                    request.Price,
                    request.BrandId,
                    request.TypeId);

                if (result.IsSuccess)
                {
                    // Save to database
                    var productRepo = unitOfWork.Repository<Product>();
                    productRepo.Add(result.Value);
                    await unitOfWork.SaveChangesAsync();

                    using (LogContext.PushProperty("CreatedProductId", result.Value.Id))
                    {
                        logger.LogInformation("Created product in database: {ProductName} (ID: {CreatedProductId}, Price: {Price})",
                            result.Value.Name,
                            result.Value.Id,
                            result.Value.Price);
                    }

                    return Results.Created($"/api/products/{result.Value.Id}", result.Value);
                }
                else
                {
                    logger.LogWarning("Validation failed for product '{ProductName}': {ErrorCode} - {ErrorMessage}",
                        request.Name,
                        result.Error?.Code,
                        result.Error?.Message);

                    return result.Error!.Type switch
                    {
                        ErrorTypes.Validation => Results.BadRequest(new { result.Error.Code, result.Error.Message }),
                        ErrorTypes.NotFound => Results.NotFound(new { result.Error.Code, result.Error.Message }),
                        ErrorTypes.Conflict => Results.Conflict(new { result.Error.Code, result.Error.Message }),
                        ErrorTypes.UnAuthorized => Results.Unauthorized(),
                        ErrorTypes.Forbidden => Results.Forbid(),
                        ErrorTypes.Failure => Results.Problem(
                            title: "Internal Server Error",
                            detail: "An error occurred. Please try again later.",
                            statusCode: 500),
                        _ => Results.Problem(
                            title: "Internal Server Error",
                            detail: "An unexpected error occurred.",
                            statusCode: 500)
                    };
                }
            }
        })
        .WithName("CreateProduct");

        // Test error endpoint
        app.MapGet("/api/test-error", () =>
        {
            logger.LogWarning("Test error endpoint called");
            throw new Exception("Test exception to demonstrate structured logging!");
        })
        .WithName("TestError");
    }
}
