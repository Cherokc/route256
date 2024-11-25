using ProductService.Domain.Dao;
using ProductService.Domain.Repository;
using ProductService.Domain.Exceptions;
using System.Collections.Concurrent;

namespace ProductService.DataAccess;

public class ProductRepository : IProductRepository
{
    private readonly ConcurrentDictionary<Guid,Product>  _products = new();

    public Guid Add(Product product)
    {
        var index = Guid.NewGuid();
        product.ChangeId(index);

        if(!_products.TryAdd(index, product))
            throw new AlreadyExistsException();

        return index;
    }

    public IReadOnlyList<Product> FindBy(ProductCategory category, DateTime creationDate, int warehouseId)
    {
        var filteredProducts = _products
            .Values
            .Where(product => IsSatisfied(product, category, creationDate, warehouseId))
            .ToList();

        return filteredProducts;
    }

    public Product Find(Guid id)
    {
        if (!_products.ContainsKey(id))
            throw new NotFoundException();

        return _products[id];
    }

    public Guid UpdatePrice(Guid id, double newPrice)
    {
        var product = Find(id);
        product.ChangePrice(newPrice);

        return product.Id;
    }

    private bool IsSatisfied(Product product, ProductCategory category, DateTime creationDate, int warehouseId)
    {
        if (category != ProductCategory.Unspecified && product.Category != category)
            return false;
        if (creationDate != DateTime.MinValue && product.CreationDate != creationDate)
            return false;
        if (warehouseId != 0 && product.WarehouseId != warehouseId)
            return false;

        return true;
    }
}
