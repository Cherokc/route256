using Bogus;
using Google.Protobuf.WellKnownTypes;
using ProductGrpcService;
using ProductService.Domain.Dao;
using ProductService.WebApi.Controllers.Dao;

namespace ProductService.IntegrationTests.ProductGrpcServiceTests.Fakers;

public class GetProductsRequestFaker : Faker<GetProductsRequest>
{
    public GetProductsRequestFaker(bool isDefaultFaker = false)
    {
        if(isDefaultFaker)
        {
            RuleFor(p => p.Category, f => ProductGrpcService.ProductCategory.ProductUnspecified);
            RuleFor(p => p.CreationDate, f => null);
            RuleFor(p => p.WarehouseId, f => 0);
            RuleFor(p => p.PageSize, f => 10);
            RuleFor(p => p.Cursor, f => "");
        }
        else
        {
            RuleFor(p => p.Category, f => f.PickRandom<ProductGrpcService.ProductCategory>());
            RuleFor(p => p.CreationDate, f => Timestamp.FromDateTime(f.Date.Past(1).ToUniversalTime()));
            RuleFor(p => p.WarehouseId, f => f.Random.Int(1, 1000));
            RuleFor(p => p.PageSize, f => f.Random.Int(1, 1000));
            RuleFor(p => p.Cursor, f => Guid.NewGuid().ToString());
        }
    }
}