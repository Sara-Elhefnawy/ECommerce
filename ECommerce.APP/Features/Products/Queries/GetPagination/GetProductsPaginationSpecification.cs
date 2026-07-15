using ECommerce.APP.Features.Products.Queries.GetAll;
using ECommerce.APP.Features.Products.Queries.GetPagination.Enums;
using ECommerce.APP.Specifications;
using ECommerce.Domain.Entities;

namespace ECommerce.APP.Features.Products.Queries.GetPagination;

public sealed class GetProductsPaginationSpecification : Specification<Product, GetAllProductsResponse>
{
    public GetProductsPaginationSpecification(
        string? searchTerm = null,
        Guid? brandId = null,
        Guid? typeId = null,
        SortType? sortBy = null,
        bool? isSortDescending = false,
        int? pageSize = null,
        int? pageNumber = null)
    {
        if (searchTerm is not null)
        {
            // Trim the search term to remove leading and trailing whitespace
            // This ensures that the search is performed on the actual content without unnecessary spaces
            // For example, if the user inputs "  laptop  ", it will be trimmed to "laptop" before searching
            // This improves the accuracy of the search results and avoids potential mismatches due to whitespace
            var trim = searchTerm.Trim();
            Query.Where(p => p.Name.Contains(trim) || p.Description.Contains(trim) || p.Brand.Name.Contains(trim) || p.Type.Name.Contains(trim));
        }

        if (brandId is Guid brandIdValue)
            Query.Where(p => p.BrandId == brandIdValue);

        if (typeId is Guid typeIdValue)
            Query.Where(p => p.TypeId == typeIdValue);

        if (sortBy is not null)
        {
            switch (sortBy)
            {
                case SortType.Name:
                    {
                        if (isSortDescending is true)
                            Query.OrderByDescending(p => p.Name);
                        else
                            Query.OrderBy(p => p.Name);
                        break;
                    }

                case SortType.Price:
                    {
                        if (isSortDescending is true)
                            Query.OrderByDescending(p => p.Price);
                        else
                            Query.OrderBy(p => p.Price);
                        break;
                    }

                case SortType.BrandName:
                    {
                        if (isSortDescending is true)
                            Query.OrderByDescending(p => p.Brand.Name);
                        else
                            Query.OrderBy(p => p.Brand.Name);
                        break;
                    }

                case SortType.TypeName:
                    {
                        if (isSortDescending is true)
                            Query.OrderByDescending(p => p.Type.Name);
                        else
                            Query.OrderBy(p => p.Type.Name);
                        break;
                    }
            }
        }

        if (pageNumber is int number && pageSize is int size)
        {
            var skip = (number - 1) * size;
            Query.Skip(skip)
                .Take(size);
        }

        Query.Select(p => new GetAllProductsResponse(
                    Id: p.Id,
                    Name: p.Name,
                    Description: p.Description,
                    PictureUrl: p.PictureUrl,
                    Price: p.Price,
                    TypeName: p.Type.Name,
                    BrandName: p.Brand.Name
                    ));
    }
}
