using ProductService.Domain.Dao;

namespace ProductService.WebApi.Controllers.Dao;
public class SelectByResponse
{
    public IEnumerable<Product> List { get; set; }
    public string Next { get; set; }
}
