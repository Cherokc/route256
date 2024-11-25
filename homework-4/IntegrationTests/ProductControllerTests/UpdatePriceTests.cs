using FluentAssertions;
using ProductService.WebApi.Controllers.Dao;
using System.Net;
using System.Net.Http.Json;

namespace ProductService.IntegrationTests.ProductControllerTests;

public class UpdatePriceTests : IntegrationTestBase
{
    public UpdatePriceTests(CustomWebFactory<Program> factory) : base(factory) { }

    [Fact]
    public async Task UpdatePrice_ShouldReturnId_WhenValidData()
    {
        // Arrange
        var request = new UpdatePriceRequest() { Id = CustomWebFactory<Program>.TestId, NewPrice = 100 };
        _factory.ProductRepositoryMock
            .Setup(repo => repo.UpdatePrice(request.Id, request.NewPrice))
            .Returns(request.Id);

        // Act
        var response = await _client.PatchAsJsonAsync("/api/v1/product/update-price", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var createdProduct = await response.Content.ReadFromJsonAsync<Guid>();
        createdProduct.Should().Be(request.Id);
    }

    [Fact]
    public async Task UpdatePrice_ShouldReturnBadRequest_WhenInvalidNewPrice()
    {
        // Arrange
        var request = new UpdatePriceRequest() { Id = CustomWebFactory<Program>.TestId, NewPrice = 0 };

        // Act
        var response = await _client.PatchAsJsonAsync("/api/v1/product/update-price", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}