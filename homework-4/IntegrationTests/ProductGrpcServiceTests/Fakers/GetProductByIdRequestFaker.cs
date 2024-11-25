using Bogus;
using ProductGrpcService;

namespace ProductService.IntegrationTests.ProductGrpcServiceTests.Fakers;

public class GetProductByIdRequestFaker : Faker<GetProductByIdRequest>
{
    public GetProductByIdRequestFaker()
    {
        RuleFor(p => p.Id, f => Guid.NewGuid().ToString());
    }
    public GetProductByIdRequestFaker(Guid id)
    {
        RuleFor(p => p.Id, f => id.ToString());
    }
}