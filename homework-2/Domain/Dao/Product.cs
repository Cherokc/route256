using ProductService.Domain.Exceptions;

namespace ProductService.Domain.Dao;

public class Product
{
    public Product(string name, double price, double weight, ProductCategory category, DateTime date, int warehouseId)
    {
        Name = name;
        Price = price;
        Weight = weight;
        Category = category;
        CreationDate = date;
        WarehouseId = warehouseId;
    }

    public Product(Guid id, string name, double price, double weight, ProductCategory category, DateTime date, int warehouseId)
    {
        Id = id;
        Name = name;
        Price = price;
        Weight = weight;
        Category = category;
        CreationDate = date;
        WarehouseId = warehouseId;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public double Price { get; private set; }
    public double Weight { get; private set; }
    public ProductCategory Category { get; private set; }
    public DateTime CreationDate { get; private set; }
    public int WarehouseId { get; private set; }

    public void ChangeId(Guid id)
    {
        if (Id != default(Guid))
            throw new ProductIdModificationException("Changing Id isnt allowed");    

        Id = id;
    }

    public void ChangePrice(double price)
    {
        if(Price <= 0)
            throw new ProductIdModificationException("Price must be greater than zero");

        Price = price;
    }
}
