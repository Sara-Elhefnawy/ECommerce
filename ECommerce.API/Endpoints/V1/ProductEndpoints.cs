using ECommerce.API.Common;
using ECommerce.API.Extensions;
using ECommerce.API.Result;
using ECommerce.APP.Products.Commands;
using ECommerce.APP.Products.Queries;
using ECommerce.APP.Products.Responses;
using Microsoft.AspNetCore.Mvc;
using Serilog.Context;

namespace ECommerce.API.Endpoints.V1;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();

        var group = app.MapVersionedEndpoint("products", ApiVersions.V1);

        group.MapGet("/", async (GetAllProductsQuery query, HttpContext httpContext, CancellationToken ct) =>
        {
            logger.LogInformation("Retrieving all products from database");

            var result = await query.Execute(ct);

            logger.LogInformation("Query completed with result: {Result}", result);

            return result.ToApiResult(httpContext);
        })
            .WithName("GetProducts")
            .WithGroupName("v1")
            .Produces<ApiResponse<IReadOnlyList<GetAllProductsResponse>>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get products")
            .WithDescription("Returns all products in DB, or 404 if list is empty");

        group.MapGet("/{id:guid}", async (Guid id, DetailsProductQuery query, HttpContext httpContext, CancellationToken ct) =>
        {
            // It's a Serilog feature, not part of your Result pattern at all.
            // Inside the using block, every log line written automatically
            // gets a ProductId field attached in the structured log output
            using (LogContext.PushProperty("ProductId", id))
            {
                var result = await query.Execute(id, ct);
                return result.ToApiResult(httpContext);
            }
        })
            .WithName("GetProductById")
            .WithGroupName("v1")
            .Produces<ApiResponse<DetailsProductResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get product by id")
            .WithDescription("Returns a single product's details, or 404 if the id doesn't exist or the product was soft-deleted.");

        group.MapPost("/", async (
            [FromForm] CreateProductRequest request,  // Use [FromForm] for multipart/form-data
            HttpContext httpContext, 
            CreateProductCommand command, 
            CancellationToken ct) =>
        {
            var result = await command.Execute(request, ct);

            // Pass location for 201 Created response
            var location = result.IsSuccess
                ? $"/api/v1/products/{result.Value.Id}"
                : null;

            return result.ToApiResult(location);
        })
            .WithName("CreateProduct")
            .WithGroupName("v1")
            .Produces<ApiResponse<CreateProductResponse>>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create  product")
            .WithDescription("Create product in DB, or 400 if validation fails or if BrandId/TypeId don't reference existing records")
            .Accepts<CreateProductRequest>("multipart/form-data")
            .DisableAntiforgery();
    }
}
