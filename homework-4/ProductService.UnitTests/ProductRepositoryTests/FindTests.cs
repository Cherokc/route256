using ProductService.DataAccess;
using ProductService.Domain.Dao;
using FluentAssertions;
using ProductService.UnitTests.ProductFakers;
using ProductService.Domain.Exceptions;

namespace ProductService.UnitTests;

public class FindTests
{
    [Fact]
    public void Find_AfterNotFinding_ShouldThrowNotFoundException()
    {
        // Arrange
        var repository = new ProductRepository();

        // Act
        Action findingAction = () =>
        {
            var foundProduct = repository.Find(Guid.NewGuid());
        };

        // Assert
        findingAction.Should().Throw<NotFoundException>();
    }

    [Fact]
    public void FindBy_WithDefaultFilters_ShouldReturnAllProducts()
    {
        // Arrange
        var products = new ValidProductFaker().Generate(10);
        var repository = new ProductRepository();
        products.ForEach(product => repository.Add(product));

        // Act
        var foundProducts = repository.FindBy(default(ProductCategory), default(DateTime), default(int));

        // Assert
        foundProducts.Should().NotBeNull();
        foundProducts.Should().BeEquivalentTo(products);
    }
}