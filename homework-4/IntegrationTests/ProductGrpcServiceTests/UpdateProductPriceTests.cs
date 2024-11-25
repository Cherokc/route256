using FluentAssertions;
using Moq;
using ProductGrpcService;
using ProductService.IntegrationTests.ProductGrpcServiceTests.Helpers;
using ProductService.WebApi.Exceptions;
using ProductService.WebApi.Validators.Grpc;


namespace ProductService.IntegrationTests.ProductGrpcServiceTests;

public class UpdateProductPriceTests : IntegrationTestBase
{
    public UpdateProductPriceTests(CustomWebFactory<Program> factory) : base(factory) { }

    [Fact]
    public async Task UpdatePrice_ShouldReturnId_WhenValidData()
    {
        // Arrange
        var service = new WebApi.GrpcServices.ProductService(_factory.ProductRepositoryMock.Object,
            new CreateProductRequestValidator(),
            new GetProductsRequestValidator(),
            new GetProductByIdRequestValidator(),
            new UpdateProductPriceRequestValidator());

        var request = new UpdateProductPriceRequest() { Id = CustomWebFactory<Program>.TestId.ToString(), NewPrice = 100 };

        _factory.ProductRepositoryMock
            .Setup(repo => repo.UpdatePrice(It.IsAny<Guid>(), It.IsAny<double>()))
            .Returns(CustomWebFactory<Program>.TestId);

        // Act
        var response = await service.UpdateProductPrice(request, TestServerCallContext.Create());

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().BeEquivalentTo(request.Id);
    }

    [Fact]
    public async Task UpdatePrice_ShouldReturnBadRequest_WhenInvalidNewPrice()
    {
        // Arrange
        var service = new WebApi.GrpcServices.ProductService(_factory.ProductRepositoryMock.Object,
            new CreateProductRequestValidator(),
            new GetProductsRequestValidator(),
            new GetProductByIdRequestValidator(),
            new UpdateProductPriceRequestValidator());

        var request = new UpdateProductPriceRequest() { Id = CustomWebFactory<Program>.TestId.ToString(), NewPrice = -100 };

        // Act
        var responseAction = async () => await service.UpdateProductPrice(request, TestServerCallContext.Create());

        // Assert
        await responseAction.Should().ThrowExactlyAsync<BadRequestException>();
    }
}