namespace SalesService.Domain;

public record Product
{
    public string Id { get; set; }
    public int Prediction { get; set; }
    public int Stock { get; set; }
    public int Demand { get; set; }
}
