namespace ProductService.IntegrationTests;

public class IntegrationTestBase : IClassFixture<CustomWebFactory<Program>>
{
    protected readonly HttpClient _client;
    protected readonly CustomWebFactory<Program> _factory;

    public IntegrationTestBase(CustomWebFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
}
