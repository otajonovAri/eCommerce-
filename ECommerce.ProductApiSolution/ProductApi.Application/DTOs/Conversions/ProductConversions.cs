using ProductApi.Domain.Models;

namespace ProductApi.Application.DTOs.Conversions;

public static class ProductConversions
{
    public static Product ToEntity(ProductDto product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Quantity = product.Quantity,
        Price = product.Price
    };

    public static (ProductDto?, IEnumerable<ProductDto>?) FromEntity(Product product , IEnumerable<Product>? products)
    {
        // return single
        if (product is null || products is null)
        {
            var singleProduct = new ProductDto
            (
                product!.Id,
                product.Name,
                product.Quantity,
                product.Price
                );

            return (singleProduct , null);
        }
        
        // return list
        if (products is not null || products is null)
        {
            var _products = products!.Select(x =>
                new ProductDto(x.Id , x.Name!, x.Quantity, x.Price));
            
            return (null , _products);
        }

        return (null, null);
    }
}
