namespace ProductService.WebApi.Controllers.Dao;

public class UpdatePriceRequest
{
    public Guid Id { get; set; }
    public double NewPrice { get; set; }
}