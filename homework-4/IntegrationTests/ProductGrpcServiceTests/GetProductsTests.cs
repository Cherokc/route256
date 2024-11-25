using FluentAssertions;
using Google.Protobuf.WellKnownTypes;
using Moq;
using ProductService.Domain.Exceptions;
using ProductService.IntegrationTests.ProductControllerTests.Fakers;
using ProductService.IntegrationTests.ProductGrpcServiceTests.Fakers;
using ProductService.IntegrationTests.ProductGrpcServiceTests.Helpers;
using ProductService.WebApi.Exceptions;
using ProductService.WebApi.Validators.Grpc;
using ProductService.WebApi.Mappers;

namespace ProductService.IntegrationTests.ProductGrpcServiceTests;

public class GetProductsTests : IntegrationTestBase
{
    public GetProductsTests(CustomWebFactory<Program> factory) : base(factory) { }

    [Fact]
    public async Task GetProducts_ShouldReturnAllProducts_WhenRequestParametersIsDefault()
    {
        // Arrange
        var service = new WebApi.GrpcServices.ProductService(_factory.ProductRepositoryMock.Object,
            new CreateProductRequestValidator(),
            new GetProductsRequestValidator(),
            new GetProductByIdRequestValidator(),
            new UpdateProductPriceRequestValidator());

        var products = new ValidProductFaker().Generate(10);
        _factory.ProductRepositoryMock.Setup(repo => repo.FindBy(0, DateTime.MinValue, 0))
            .Returns(products);
        var request = new GetProductsRequestFaker(isDefaultFaker: true).Generate(); 

        // Act
        var response = await service.GetProducts(request, TestServerCallContext.Create());

        // Assert
        response.Should().NotBeNull();
        var p = response.Products.ToList();
        var pp = products.Select(ProductMapper.ChangeToProtoProduct);
        p.Should().BeEquivalentTo(pp);
    }

    [Fact]
    public async Task GetProducts_ShouldThrowBadRequestException_WhenRequestCategoryIsInvalid()
    {
        // Arrange
        var service = new WebApi.GrpcServices.ProductService(_factory.ProductRepositoryMock.Object,
            new CreateProductRequestValidator(),
            new GetProductsRequestValidator(),
            new GetProductByIdRequestValidator(),
            new UpdateProductPriceRequestValidator());

        var request = new GetProductsRequestFaker().Generate();
        request.Category = (ProductGrpcService.ProductCategory)System.Enum.GetValues(typeof(ProductGrpcService.ProductCategory)).Length + 1;

        // Act
        var responseAction = async () => await service.GetProducts(request, TestServerCallContext.Create());

        // Assert
        await responseAction.Should().ThrowExactlyAsync<BadRequestException>();
    }

    [Fact]
    public async Task GetProducts_ShouldThrowBadRequestException_WhenRequestCreationDateIsInvalid()
    {
        // Arrange
        var service = new WebApi.GrpcServices.ProductService(_factory.ProductRepositoryMock.Object,
            new CreateProductRequestValidator(),
            new GetProductsRequestValidator(),
            new GetProductByIdRequestValidator(),
            new UpdateProductPriceRequestValidator());

        var request = new GetProductsRequestFaker(isDefaultFaker: false).Generate();
        request.CreationDate = Timestamp.FromDateTime(DateTime.UtcNow.AddDays(1));

        // Act
        var responseAction = async () => await service.GetProducts(request, TestServerCallContext.Create());

        // Assert
        await responseAction.Should().ThrowExactlyAsync<BadRequestException>();
    }

    [Fact]
    public async Task GetProducts_ShouldThrowBadRequestException_WhenRequestWarehouseIdIsInvalid()
    {
        // Arrange
        var service = new WebApi.GrpcServices.ProductService(_factory.ProductRepositoryMock.Object,
            new CreateProductRequestValidator(),
            new GetProductsRequestValidator(),
            new GetProductByIdRequestValidator(),
            new UpdateProductPriceRequestValidator());

        var request = new GetProductsRequestFaker().Generate();
        request.WarehouseId = -1;

        // Act
        var responseAction = async () => await service.GetProducts(request, TestServerCallContext.Create());

        // Assert
        await responseAction.Should().ThrowExactlyAsync<BadRequestException>();
    }

    [Fact]
    public async Task GetProducts_ShouldThrowBadRequestException_WhenRequestCursorIsInvalid()
    {
        // Arrange
        var service = new WebApi.GrpcServices.ProductService(_factory.ProductRepositoryMock.Object,
            new CreateProductRequestValidator(),
            new GetProductsRequestValidator(),
            new GetProductByIdRequestValidator(),
            new UpdateProductPriceRequestValidator());

        _factory.ProductRepositoryMock.Setup(repo => repo.FindBy(It.IsAny<Domain.Dao.ProductCategory>(), It.IsAny<DateTime>(), It.IsAny<int>()))
            .Throws(new NotFoundException());
        var request = new GetProductsRequestFaker().Generate();
        request.Cursor = Guid.NewGuid().ToString();

        // Act
        var responseAction = async () => await service.GetProducts(request, TestServerCallContext.Create());

        // Assert
        await responseAction.Should().ThrowExactlyAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetProducts_ShouldThrowBadRequestException_WhenRequestPageSizeIsInvalid()
    {
        // Arrange
        var service = new WebApi.GrpcServices.ProductService(_factory.ProductRepositoryMock.Object,
            new CreateProductRequestValidator(),
            new GetProductsRequestValidator(),
            new GetProductByIdRequestValidator(),
            new UpdateProductPriceRequestValidator());

        var request = new GetProductsRequestFaker().Generate();
        request.PageSize = -1;

        // Act
        var responseAction = async () => await service.GetProducts(request, TestServerCallContext.Create());

        // Assert
        await responseAction.Should().ThrowExactlyAsync<BadRequestException>();
    }
}