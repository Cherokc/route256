using FluentAssertions;
using Moq;
using ProductService.Domain.Exceptions;
using ProductService.IntegrationTests.ProductControllerTests.Fakers;
using System.Net;
using System.Net.Http.Json;

namespace ProductService.IntegrationTests.ProductControllerTests;

public class FindByIdTests : IntegrationTestBase
{
    public FindByIdTests(CustomWebFactory<Program> factory) : base(factory) { }

    [Fact]
    public async Task FindById_ShouldReturnOk_WhenProductExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var product = new ValidProductWithIdFaker(id).Generate();
        _factory.ProductRepositoryMock.Setup(repo => repo.Find(id))
            .Returns(product);

        // Act
        var response = await _client.GetAsync("/api/v1/product/find-by-id?id=" + id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var foundProduct = await response.Content.ReadFromJsonAsync<Domain.Dao.Product>();
        foundProduct.Should().NotBeNull();
        foundProduct.Should().BeEquivalentTo(product);
    }

    [Fact]
    public async Task FindById_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        _factory.ProductRepositoryMock
            .Setup(repo => repo.Find(It.IsAny<Guid>()))
            .Throws(new NotFoundException());

        // Act
        var response = await _client.GetAsync($"/api/v1/product/find-by-id?id={Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}