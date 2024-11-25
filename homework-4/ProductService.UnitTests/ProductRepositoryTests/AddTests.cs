using ProductService.DataAccess;
using FluentAssertions;
using ProductService.UnitTests.ProductFakers;
using ProductService.Domain.Exceptions;

namespace ProductService.UnitTests;

public class AddTests
{
    [Fact]
    public void Add_AfterAddingProduct_ShouldReturnIdOfAdded()
    {
        // Arrange
        var product = new ValidProductFaker().Generate();
        var repository = new ProductRepository();

        // Act
        var productId = repository.Add(product);

        // Assert
        productId.Should().NotBeEmpty();
        var addedProduct = repository.Find(productId);
        addedProduct.Id.Should().Be(product.Id);
    }

    [Fact]
    public void Add_AfterAddingProductWithId_ShouldThrowProductModificationException()
    {
        // Arrange
        var product = new ValidProductWithIdFaker().Generate();
        var repository = new ProductRepository();

        // Act
        Action addingAction = () =>
        {
            repository.Add(product);
        };

        // Assert
        addingAction.Should().Throw<ProductModificationException>();
    }
}