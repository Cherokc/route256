using ProductService.Domain.Dao;

namespace ProductService.WebApi.Controllers.Dao;

public class ProductDto
{
    public string Name { get; set; }
    public double Price { get; set; }
    public double Weight { get; set; }
    public ProductCategory Category { get; set; }
    public DateTime CreationDate { get; set; }
    public int WarehouseId { get; set; }
}
