using ProductService.Domain.Dao;

namespace ProductService.WebApi.Controllers.Dao.Extensions;

public static class ProductDtoExtensions
{
    public static Product ConvertToDomainProduct(this ProductDto product) 
    {
        return new Product(
            product.Name,
            product.Price,
            product.Weight,
            product.Category,
            product.CreationDate,
            product.WarehouseId);
    }
}
