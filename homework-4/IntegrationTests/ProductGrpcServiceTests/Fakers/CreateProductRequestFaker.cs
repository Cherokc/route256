using Bogus;
using Google.Protobuf.WellKnownTypes;
using ProductGrpcService;
using ProductService.Domain.Dao;
using ProductService.WebApi.Controllers.Dao;

namespace ProductService.IntegrationTests.ProductGrpcServiceTests.Fakers;

public class CreateProductRequestFaker : Faker<CreateProductRequest>
{
    public CreateProductRequestFaker()
    {
        RuleFor(p => p.Name, f => f.Commerce.ProductName());
        RuleFor(p => p.Price, f => f.Random.Double(1, 1000));
        RuleFor(p => p.Weight, f => f.Random.Double(1, 1000));
        RuleFor(p => p.Category, f => f.PickRandom<ProductGrpcService.ProductCategory>());
        RuleFor(p => p.CreationDate, f => Timestamp.FromDateTime(f.Date.Past(1).ToUniversalTime()));
        RuleFor(p => p.WarehouseId, f => f.Random.Int(1, 1000));
    }
}