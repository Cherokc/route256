using Bogus;
using ProductService.Domain.Dao;
using ProductService.WebApi.Controllers.Dao;

namespace ProductService.IntegrationTests.ProductControllerTests.Fakers;

public class DefaultProductFilterFaker : Faker<Filter>
{
    public DefaultProductFilterFaker()
    {
        RuleFor(p => p.Cursor, f => Guid.Empty);
        RuleFor(p => p.PageSize, f => 10);
        RuleFor(p => p.Category, f => ProductCategory.Unspecified);
        RuleFor(p => p.CreationDate, f => DateTime.MinValue);
        RuleFor(p => p.WarehouseId, f => 0);
    }
}