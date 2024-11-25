using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ProductService.DataAccess;
using ProductService.Domain.Repository;

namespace ProductService.IntegrationTests;

public class CustomWebFactory<TProgram> : WebApplicationFactory<TProgram>, IDisposable where TProgram : class
{
    public readonly Mock<IProductRepository> ProductRepositoryMock = new Mock<IProductRepository>(MockBehavior.Loose);
    public static readonly Guid TestId = Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(Services =>
        {
            var descriptor = Services.SingleOrDefault(ietm => ietm.ServiceType == typeof(ProductRepository));
            Services.Remove(descriptor);

            var mockProductRepository = new Mock<IProductRepository>();

            Services.AddSingleton(ProductRepositoryMock.Object);
        });
    }

    protected override void Dispose(bool disposing)
    {
        ProductRepositoryMock.Reset();
        base.Dispose(disposing);
    }
}
