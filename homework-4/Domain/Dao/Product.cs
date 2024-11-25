using ProductService.Domain.Exceptions;

namespace ProductService.Domain.Dao;

public class Product
{
    public Product() { }

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

    public Guid Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public double Weight { get; set; }
    public ProductCategory Category { get; set; }
    public DateTime CreationDate { get; set; }
    public int WarehouseId { get; set; }

    public void ChangeId(Guid id)
    {
        if (Id != default(Guid))
            throw new ProductModificationException("Changing Id isnt allowed");    

        Id = id;
    }

    public void ChangePrice(double price)
    {
        if(price <= 0)
            throw new ProductModificationException("Price must be greater than zero");

        Price = price;
    }
}
