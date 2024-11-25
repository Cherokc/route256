using Bogus;
using ProductService.Domain.Dao;
using ProductService.WebApi.Controllers.Dao;

namespace ProductService.IntegrationTests.ProductControllerTests.Fakers;

public class ValidProductFaker : Faker<Product>
{
    public ValidProductFaker()
    {
        RuleFor(p => p.Name, f => f.Commerce.ProductName());
        RuleFor(p => p.Price, f => f.Random.Double(1, 1000));
        RuleFor(p => p.Weight, f => f.Random.Double(1, 1000));
        RuleFor(p => p.Category, f => f.PickRandom<ProductCategory>());
        RuleFor(p => p.CreationDate, f => f.Date.Past(1));
        RuleFor(p => p.WarehouseId, f => f.Random.Int(1, 1000));
    }
}