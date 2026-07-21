using ECommerce.Domain.Entities;
using ECommerce.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistent.Seedings;

public class ProductSeeder(ECommerceDbContext dbContext) : IDataSeeder
{
    public int Order => 3;

    public async Task SeedAsync(CancellationToken ct = default)
    {
        if (await dbContext.Products.AnyAsync(ct))
            return;

        // Get brand IDs by name
        var brandIds = await dbContext.Brands
            .Select(b => new { b.Name, b.Id })
            .ToListAsync(ct);
        var brandIdMap = brandIds.ToDictionary(x => x.Name, x => x.Id);

        // Get type IDs by name
        var typeIds = await dbContext.Types
            .Select(t => new { t.Name, t.Id })
            .ToListAsync(ct);
        var typeIdMap = typeIds.ToDictionary(x => x.Name, x => x.Id);

        var types = await dbContext.Types.ToDictionaryAsync(t => t.Name, t => t, ct);

        // Verify all required IDs exist
        var requiredBrands = new[] { "H&M", "ZARA", "Nike", "Activ", "Mango", "Levi's" };
        foreach (var brand in requiredBrands)
        {
            if (!brandIdMap.ContainsKey(brand))
                throw new InvalidOperationException($"Brand '{brand}' not found. Make sure ProductBrandSeeder ran successfully.");
        }

        var requiredTypes = new[] { "Tops", "Bottoms", "Outerwear & Dresses", "Accessories & Footwear" };
        foreach (var type in requiredTypes)
        {
            if (!typeIdMap.ContainsKey(type))
                throw new InvalidOperationException($"Type '{type}' not found. Make sure ProductTypeSeeder ran successfully.");
        }

        // DIRECT COLLECTION INITIALIZATION
        // 1. Create all products with their names for error reporting
        // 2. Check for any failures in one go
        // 3. If any failed, throw a comprehensive error message
        // 4. If all succeeded, extract the products and add to database

        var productCreations = new (string Name, ResultOfT<Product> Result)[]
        {
            (
                "Classic White T-Shirt",
                Product.Create(
                    "Classic White T-Shirt",
                    "A soft cotton white t-shirt that goes with everything. Perfect for casual everyday wear.",
                    "images/products/ClassicWhiteTShirt.jpeg",
                    120,
                    brandIdMap["H&M"],
                    typeIdMap["Tops"]
                )
            ),
            (
                "Slim Fit Jeans",
                Product.Create(
                    "Slim Fit Jeans",
                    "These mid-rise slim fit jeans offer a comfortable stretch and a stylish look for all occasions.",
                    "images/products/SlimFitJeans.jpg",
                    350,
                    brandIdMap["ZARA"],
                    typeIdMap["Bottoms"]
                )
            ),
            (
                "Denim Jacket",
                Product.Create(
                    "Denim Jacket",
                    "A timeless denim jacket with a slightly faded wash. Layer it over any outfit for an instant upgrade.",
                    "images/products/DenimJacket.jpg",
                    500,
                    brandIdMap["ZARA"],
                    typeIdMap["Outerwear & Dresses"]
                )
            ),
            (
                "Cotton Hoodie",
                Product.Create(
                    "Cotton Hoodie",
                    "Soft and cozy cotton hoodie with adjustable drawstring and kangaroo pocket. Ideal for cool evenings.",
                    "images/products/CottonHoodie.jpg",
                    400,
                    brandIdMap["Nike"],
                    typeIdMap["Tops"]
                )
            ),
            (
                "Formal Blazer",
                Product.Create(
                    "Formal Blazer",
                    "Tailored-fit blazer that adds a touch of sophistication to any outfit. Perfect for business or evening wear.",
                    "images/products/FormalBlazer.jpg",
                    850,
                    brandIdMap["Activ"],
                    typeIdMap["Outerwear & Dresses"]
                )
            ),
            (
                "Summer Floral Dress",
                Product.Create(
                    "Summer Floral Dress",
                    "Flowy floral summer dress with adjustable straps and a flattering waistline. Breathable and comfortable.",
                    "images/products/SummerFloralDress.jpg",
                    450,
                    brandIdMap["Mango"],
                    typeIdMap["Outerwear & Dresses"]
                )
            ),
            (
                "Men's Leather Jacket",
                Product.Create(
                    "Men's Leather Jacket",
                    "Classic brown leather jacket with zip closure and side pockets. Durable and stylish for all seasons.",
                    "images/products/MensLeatherJacket.jpg",
                    1200,
                    brandIdMap["Levi's"],
                    typeIdMap["Outerwear & Dresses"]
                )
            ),
            (
                "Casual Sneakers",
                Product.Create(
                    "Casual Sneakers",
                    "Lightweight sneakers designed for everyday wear. Breathable material with cushioned soles.",
                    "images/products/CasualSneakers.jpg",
                    600,
                    brandIdMap["Nike"],
                    typeIdMap["Accessories & Footwear"]
                )
            ),
            (
                "Classic Black Dress Pants",
                Product.Create(
                    "Classic Black Dress Pants",
                    "Slim-cut black dress pants that offer a polished look for formal or office wear.",
                    "images/products/ClassicBlackDressPants.jpg",
                    400,
                    brandIdMap["H&M"],
                    typeIdMap["Bottoms"]
                )
            ),
            (
                "Knitted Sweater",
                Product.Create(
                    "Knitted Sweater",
                    "Warm knitted sweater with a crew neckline and ribbed cuffs. Ideal for layering in winter.",
                    "images/products/KnittedSweater.jpg",
                    380,
                    brandIdMap["ZARA"],
                    typeIdMap["Tops"]
                )
            ),
            (
                "Men's Polo Shirt",
                Product.Create(
                    "Men's Polo Shirt",
                    "Classic short-sleeve polo shirt with a soft feel and neat collar. Perfect for casual Fridays.",
                    "images/products/MensPoloShirt.jpg",
                    200,
                    brandIdMap["Activ"],
                    typeIdMap["Tops"]
                )
            ),
            (
                "Women's Trench Coat",
                Product.Create(
                    "Women's Trench Coat",
                    "Chic beige trench coat with adjustable belt and water-resistant fabric. A wardrobe essential.",
                    "images/products/WomensTrenchCoat.jpeg",
                    950,
                    brandIdMap["Mango"],
                    typeIdMap["Outerwear & Dresses"]
                )
            ),
            (
                "Wool Scarf",
                Product.Create(
                    "Wool Scarf",
                    "Soft wool scarf that keeps you warm while adding a stylish touch to your winter outfits.",
                    "images/products/WoolScarf.jpg",
                    150,
                    brandIdMap["ZARA"],
                    typeIdMap["Accessories & Footwear"]
                )
            )
        };

        // Check for any failures
        var failedProducts = productCreations
            .Where(p => p.Result.IsFailure)
            .ToList();

        if (failedProducts.Any())
        {
            // Build a comprehensive error message
            var errorMessages = failedProducts.Select(p =>
                $"{p.Name}: {p.Result.Error!.Code} - {p.Result.Error!.Message}");

            throw new InvalidOperationException(
                $"Failed to create {failedProducts.Count} product(s):{Environment.NewLine}" +
                string.Join(Environment.NewLine, errorMessages));
        }

        // All succeeded - extract the products
        var products = productCreations
            .Select(p => p.Result.Value)
            .ToList();

        await dbContext.Products.AddRangeAsync(products, ct);
    }
}
