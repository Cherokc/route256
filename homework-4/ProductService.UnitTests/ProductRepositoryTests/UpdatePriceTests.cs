using ProductService.DataAccess;
using FluentAssertions;
using ProductService.UnitTests.ProductFakers;
using ProductService.Domain.Exceptions;

namespace ProductService.UnitTests;

public class UpdatePriceTests
{
    [Fact]
    public void UpdatePrice_WithValidIdAndNewPrice_ShouldReturnSameIdAndChangePrice()
    {
        // Arrange
        var product = new ValidProductFaker().Generate();
        var repository = new ProductRepository();
        var id = repository.Add(product);
        var newPrice = product.Price > 2 ? product.Price - 1 : product.Price + 1;

        // Act
        var changedProductId = repository.UpdatePrice(id, newPrice);

        // Assert
        changedProductId.Should().NotBeEmpty();
        changedProductId.Should().Be(id);
        product.Price.Should().Be(newPrice);
    }

    [Fact]
    public void UpdatePrice_WithInvalidId_ShouldThrowNotFoundException()
    {
        // Arrange
        var product = new ValidProductFaker().Generate();
        var repository = new ProductRepository();

        // Act
        Action changingPriceAction = () =>
        {
            var changedProductId = repository.UpdatePrice(product.Id, product.Price);
        };

        // Assert
        changingPriceAction.Should().Throw<NotFoundException>();
    }

    [Fact]
    public void UpdatePrice_WithInvalidNewPrice_ShouldThrowProductModificationException()
    {
        // Arrange
        var product = new ValidProductFaker().Generate();
        var repository = new ProductRepository();
        var id = repository.Add(product);
        var invalidNewPrice = 0;

        // Act
        Action changingPriceAction = () =>
        {
            var changedProductId = repository.UpdatePrice(product.Id, invalidNewPrice);
        };

        // Assert
        changingPriceAction.Should().Throw<ProductModificationException>();
    }
}