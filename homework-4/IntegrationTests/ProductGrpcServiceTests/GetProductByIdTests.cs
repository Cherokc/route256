using FluentAssertions;
using Moq;
using ProductService.Domain.Dao;
using ProductService.Domain.Exceptions;
using ProductService.IntegrationTests.ProductGrpcServiceTests.Fakers;
using ProductService.IntegrationTests.ProductGrpcServiceTests.Helpers;
using ProductService.WebApi.Validators.Grpc;

namespace ProductService.IntegrationTests.ProductGrpcServiceTests;

public class GetProductByIdTests : IntegrationTestBase
{
    public GetProductByIdTests(CustomWebFactory<Program> factory) : base(factory) { }

    [Fact]
    public async Task GetProductById_ShouldReturnProductWithSameId_WhenProductExists()
    {
        // Arrange
        var service = new WebApi.GrpcServices.ProductService(_factory.ProductRepositoryMock.Object,
            new CreateProductRequestValidator(),
            new GetProductsRequestValidator(),
            new GetProductByIdRequestValidator(),
            new UpdateProductPriceRequestValidator());

        _factory.ProductRepositoryMock
            .Setup(repo => repo.Find(It.IsAny<Guid>()))
            .Returns(new DomainProductWithIdFaker(CustomWebFactory<Program>.TestId).Generate());

        var request = new GetProductByIdRequestFaker().Generate();

        // Act
        var response = await service.GetProductById(request, TestServerCallContext.Create());

        // Assert
        response.Should().NotBeNull();
        response.Product.Id.Should().BeEquivalentTo(CustomWebFactory<Program>.TestId.ToString());
    }

    [Fact]
    public async Task GetProductById_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        var service = new WebApi.GrpcServices.ProductService(_factory.ProductRepositoryMock.Object,
            new CreateProductRequestValidator(),
            new GetProductsRequestValidator(),
            new GetProductByIdRequestValidator(),
            new UpdateProductPriceRequestValidator());

        _factory.ProductRepositoryMock
            .Setup(repo => repo.Find(It.IsAny<Guid>()))
            .Throws(new NotFoundException());

        var request = new GetProductByIdRequestFaker().Generate();

        // Act
        var responseAction = async () => await service.GetProductById(request, TestServerCallContext.Create());

        // Assert
        await responseAction.Should().ThrowExactlyAsync<NotFoundException>();
    }
}