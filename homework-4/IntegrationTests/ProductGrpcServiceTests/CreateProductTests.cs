using FluentAssertions;
using Moq;
using ProductService.IntegrationTests.ProductGrpcServiceTests.Fakers;
using ProductService.IntegrationTests.ProductGrpcServiceTests.Helpers;
using ProductService.WebApi.Exceptions;
using ProductService.WebApi.Validators.Grpc;

namespace ProductService.IntegrationTests.ProductGrpcServiceTests;

public class CreateProductTests : IntegrationTestBase
{
    public CreateProductTests(CustomWebFactory<Program> factory) : base(factory) { }

    [Fact]
    public async Task CreateProduct_ShouldReturnId_WhenValidData()
    {
        // Arrange
        var service = new WebApi.GrpcServices.ProductService(_factory.ProductRepositoryMock.Object, 
            new CreateProductRequestValidator(), 
            new GetProductsRequestValidator(), 
            new GetProductByIdRequestValidator(), 
            new UpdateProductPriceRequestValidator());

        _factory.ProductRepositoryMock
            .Setup(repo => repo.Add(It.IsAny<Domain.Dao.Product>()))
            .Returns(CustomWebFactory<Program>.TestId);

        var request = new CreateProductRequestFaker().Generate();

        // Act
        var response = await service.CreateProduct(request, TestServerCallContext.Create());

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().BeEquivalentTo(CustomWebFactory<Program>.TestId.ToString());
    }

    [Fact]
    public async Task CreateProduct_ShouldThrowBadRequestException_WhenInvalidName()
    {
        // Arrange
        var service = new WebApi.GrpcServices.ProductService(_factory.ProductRepositoryMock.Object,
            new CreateProductRequestValidator(),
            new GetProductsRequestValidator(),
            new GetProductByIdRequestValidator(),
            new UpdateProductPriceRequestValidator());

        var request = new CreateProductRequestFaker().Generate();
        request.Name = "";

        // Act
        var responseAction = async () => await service.CreateProduct(request, TestServerCallContext.Create());

        // Assert
        await responseAction.Should().ThrowExactlyAsync<BadRequestException>();
    }

    [Fact]
    public async Task CreateProduct_ShouldThrowBadRequestException_WhenInvalidPrice()
    {
        // Arrange
        var service = new WebApi.GrpcServices.ProductService(_factory.ProductRepositoryMock.Object,
            new CreateProductRequestValidator(),
            new GetProductsRequestValidator(),
            new GetProductByIdRequestValidator(),
            new UpdateProductPriceRequestValidator());

        var request = new CreateProductRequestFaker().Generate();
        request.Price = -1;

        // Act
        var responseAction = async () => await service.CreateProduct(request, TestServerCallContext.Create());

        // Assert
        await responseAction.Should().ThrowExactlyAsync<BadRequestException>();
    }

    [Fact]
    public async Task CreateProduct_ShouldThrowBadRequestException_WhenInvalidWeight()
    {
        // Arrange
        var service = new WebApi.GrpcServices.ProductService(_factory.ProductRepositoryMock.Object,
            new CreateProductRequestValidator(),
            new GetProductsRequestValidator(),
            new GetProductByIdRequestValidator(),
            new UpdateProductPriceRequestValidator());

        var request = new CreateProductRequestFaker().Generate();
        request.Weight = -1;

        // Act
        var responseAction = async () => await service.CreateProduct(request, TestServerCallContext.Create());

        // Assert
        await responseAction.Should().ThrowExactlyAsync<BadRequestException>();
    }

    [Fact]
    public async Task CreateProduct_ShouldThrowBadRequestException_WhenInvalidCategory()
    {
        // Arrange
        var service = new WebApi.GrpcServices.ProductService(_factory.ProductRepositoryMock.Object,
            new CreateProductRequestValidator(),
            new GetProductsRequestValidator(),
            new GetProductByIdRequestValidator(),
            new UpdateProductPriceRequestValidator());

        var request = new CreateProductRequestFaker().Generate();
        request.Category = (ProductGrpcService.ProductCategory)Enum.GetValues(typeof(ProductGrpcService.ProductCategory)).Length + 1;

        // Act
        var responseAction = async () => await service.CreateProduct(request, TestServerCallContext.Create());

        // Assert
        await responseAction.Should().ThrowExactlyAsync<BadRequestException>();
    }

    [Fact]
    public async Task CreateProduct_ShouldThrowBadRequestException_WhenInvalidCreationDate()
    {
        // Arrange
        var service = new WebApi.GrpcServices.ProductService(_factory.ProductRepositoryMock.Object,
            new CreateProductRequestValidator(),
            new GetProductsRequestValidator(),
            new GetProductByIdRequestValidator(),
            new UpdateProductPriceRequestValidator());

        var request = new CreateProductRequestFaker().Generate();
        request.CreationDate.Nanos = -1;

        // Act
        var responseAction = async () => await service.CreateProduct(request, TestServerCallContext.Create());

        // Assert
        await responseAction.Should().ThrowExactlyAsync<BadRequestException>();
    }

    [Fact]
    public async Task CreateProduct_ShouldThrowBadRequestException_WhenInvalidWarehouseId()
    {
        // Arrange
        var service = new WebApi.GrpcServices.ProductService(_factory.ProductRepositoryMock.Object,
            new CreateProductRequestValidator(),
            new GetProductsRequestValidator(),
            new GetProductByIdRequestValidator(),
            new UpdateProductPriceRequestValidator());

        var request = new CreateProductRequestFaker().Generate();
        request.WarehouseId = -1;

        // Act
        var responseAction = async () => await service.CreateProduct(request, TestServerCallContext.Create());

        // Assert
        await responseAction.Should().ThrowExactlyAsync<BadRequestException>();
    }
}