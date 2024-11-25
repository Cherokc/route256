using FluentAssertions;
using Moq;
using ProductService.Domain.Exceptions;
using ProductService.IntegrationTests.ProductControllerTests.Fakers;
using ProductService.WebApi.Controllers.Dao;
using System.Net;
using System.Net.Http.Json;

namespace ProductService.IntegrationTests.ProductControllerTests;

public class SelectByTests : IntegrationTestBase
{
    public SelectByTests(CustomWebFactory<Program> factory) : base(factory) { }

    [Fact]
    public async Task SelectBy_ShouldReturnOkAndAllProducts_WhenFilterParametersIsDefault()
    {
        // Arrange
        var products = new ValidProductFaker().Generate(10);
        var filter = new DefaultProductFilterFaker().Generate();
        _factory.ProductRepositoryMock.Setup(repo => repo.FindBy(0, DateTime.MinValue, 0))
            .Returns(products);

        // Act
        var response = await _client.GetAsync($"/api/v1/product/select-by?Category={filter.Category}&CreationDate={filter.CreationDate}&WarehouseId={filter.WarehouseId}&Cursor={filter.Cursor}&PageSize={filter.PageSize}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var foundProducts = await response.Content.ReadFromJsonAsync<SelectByResponse>();
        foundProducts.Should().NotBeNull();
        foundProducts.List.Should().NotBeNull();
        foundProducts.Next.Should().NotBeNull();
        foundProducts.List.Should().BeEquivalentTo(products);
    }

    [Fact]
    public async Task SelectBy_ShouldReturnBadRequest_WhenFilterCategoryIsInvalid()
    {
        // Arrange
        var category = Enum.GetValues(typeof(Domain.Dao.ProductCategory)).Length + 1;

        // Act
        var response = await _client.GetAsync($"/api/v1/product/select-by?Category={category}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SelectBy_ShouldReturnBadRequest_WhenFilterCreationDateIsInvalid()
    {
        // Arrange
        var dateTime = DateTime.Now.AddDays(1).ToString("O");

        // Act
        var response = await _client.GetAsync($"/api/v1/product/select-by?CreationDate={dateTime}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SelectBy_ShouldReturnBadRequest_WhenFilterWarehouseIdIsInvalid()
    {
        // Arrange
        var warehouseId = -1;

        // Act
        var response = await _client.GetAsync($"/api/v1/product/select-by?WarehouseId={warehouseId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SelectBy_ShouldReturnNotFound_WhenFilterCursorIsInvalid()
    {
        // Arrange
        var cursour = Guid.NewGuid();
        _factory.ProductRepositoryMock.Setup(repo => repo.Find(It.IsAny<Guid>()))
            .Throws(new NotFoundException());

        // Act
        var response = await _client.GetAsync($"/api/v1/product/select-by?Cursor={cursour}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SelectBy_ShouldReturnBadRequest_WhenFilterPageSizeIsInvalid()
    {
        // Arrange
        var pageSize = -1;

        // Act
        var response = await _client.GetAsync($"/api/v1/product/select-by?PageSize={pageSize}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}