using ProductService.Domain.Dao;

namespace ProductService.Domain.Repository;

public interface IProductRepository
{
    Guid Add(Product item);

    IReadOnlyList<Product> FindBy(ProductCategory category, DateTime creationDate, int warehouseId);

    Product Find(Guid id);

    Guid UpdatePrice(Guid id, double newPrice);
}