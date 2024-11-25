using ProductService.Domain.Dao;

namespace ProductService.WebApi.Controllers.Dao;

public class Filter
{
    public ProductCategory Category { get; set; }
    public DateTime CreationDate { get; set; }
    public int WarehouseId { get; set; }
    public Guid Cursor { get; set; }
    public int PageSize { get; set; }
}
