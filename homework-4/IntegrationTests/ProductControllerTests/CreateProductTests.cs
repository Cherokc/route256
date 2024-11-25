using FluentAssertions;
using Moq;
using ProductService.Domain.Dao;
using ProductService.IntegrationTests.ProductControllerTests.Fakers;
using System.Net;
using System.Net.Http.Json;

namespace ProductService.IntegrationTests.ProductControllerTests;

public class CreateProductTests : IntegrationTestBase
{
    public CreateProductTests(CustomWebFactory<Program> factory) : base(factory) { }

    [Fact]
    public async Task CreateProduct_ShouldReturnId_WhenValidData()
    {
        // Arrange
        var product = new ValidProductDtoFaker().Generate();
        _factory.ProductRepositoryMock
            .Setup(repo => repo.Add(It.IsAny<Product>()))
            .Returns(CustomWebFactory<Program >.TestId);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/product/create", product);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var createdProduct = await response.Content.ReadFromJsonAsync<Guid>();
        createdProduct.Should().Be(CustomWebFactory<Program>.TestId);
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnBadRequest_WhenInvalidName()
    {
        // Arrange
        var product = new ValidProductDtoFaker().Generate();
        product.Name = "";

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/product/create", product);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnBadRequest_WhenInvalidPrice()
    {
        // Arrange
        var product = new ValidProductDtoFaker().Generate();
        product.Price = 0;

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/product/create", product);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnBadRequest_WhenInvalidWeight()
    {
        // Arrange
        var product = new ValidProductDtoFaker().Generate();
        product.Weight = 0;

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/product/create", product);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnBadRequest_WhenInvalidCategory()
    {
        // Arrange
        var product = new ValidProductDtoFaker().Generate();
        product.Category = (ProductCategory)Enum.GetValues(typeof(ProductCategory)).Length + 1;

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/product/create", product);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnBadRequest_WhenInvalidCreationDate()
    {
        // Arrange
        var product = new ValidProductDtoFaker().Generate();
        product.CreationDate = DateTime.MinValue;

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/product/create", product);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnBadRequest_WhenInvalidWarehouseId()
    {
        // Arrange
        var product = new ValidProductDtoFaker().Generate();
        product.WarehouseId = 0;

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/product/create", product);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}