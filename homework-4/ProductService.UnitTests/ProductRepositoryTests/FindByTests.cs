using ProductService.DataAccess;
using ProductService.Domain.Dao;
using FluentAssertions;
using ProductService.UnitTests.ProductFakers;

namespace ProductService.UnitTests;

public class FindByTests
{
    [Fact]
    public void Find_AfterFinding_ShouldReturnProductWithSameId()
    {
        // Arrange
        var product = new ValidProductFaker().Generate();
        var repository = new ProductRepository();

        // Act
        var id = repository.Add(product);
        var foundProduct = repository.Find(id);

        // Assert
        foundProduct.Id.Should().NotBeEmpty();
        foundProduct.Id.Should().Be(product.Id);
    }

    [Fact]
    public void FindBy_WithValidFilters_ShouldReturnSpecifiedProduct()
    {
        // Arrange
        var product = new ValidProductFaker().Generate();
        var repository = new ProductRepository();
        repository.Add(product);

        // Act
        var foundProducts = repository.FindBy(product.Category, product.CreationDate, product.WarehouseId);

        // Assert
        foundProducts.Should().NotBeNull();
        var foundProduct = foundProducts.FirstOrDefault();
        foundProduct.Should().NotBeNull();
        foundProduct.Should().BeEquivalentTo(product);
    }

    [Fact]
    public void FindBy_WithValidCategoryFilter_ShouldReturnSpecifiedProduct()
    {
        // Arrange
        var product = new ValidProductFaker().Generate();
        var repository = new ProductRepository();
        repository.Add(product);

        // Act
        var foundProducts = repository.FindBy(product.Category, default(DateTime), default(int));

        // Assert
        foundProducts.Should().NotBeNull();
        var foundProduct = foundProducts.FirstOrDefault();
        foundProduct.Should().NotBeNull();
        foundProduct.Should().BeEquivalentTo(product);
    }

    [Fact]
    public void FindBy_WithValidDatetimeFilter_ShouldReturnSpecifiedProduct()
    {
        // Arrange
        var product = new ValidProductFaker().Generate();
        var repository = new ProductRepository();
        repository.Add(product);

        // Act
        var foundProducts = repository.FindBy(default(ProductCategory), product.CreationDate, default(int));

        // Assert
        foundProducts.Should().NotBeNull();
        var foundProduct = foundProducts.FirstOrDefault();
        foundProduct.Should().NotBeNull();
        foundProduct.Should().BeEquivalentTo(product);
    }

    [Fact]
    public void FindBy_WithValidWarehouseFilter_ShouldReturnSpecifiedProduct()
    {
        // Arrange
        var product = new ValidProductFaker().Generate();
        var repository = new ProductRepository();
        repository.Add(product);

        // Act
        var foundProducts = repository.FindBy(default(ProductCategory), default(DateTime), product.WarehouseId);

        // Assert
        foundProducts.Should().NotBeNull();
        var foundProduct = foundProducts.FirstOrDefault();
        foundProduct.Should().NotBeNull();
        foundProduct.Should().BeEquivalentTo(product);
    }

    [Fact]
    public void FindBy_WithInvaliFilters_ShouldReturnEmptyCollection()
    {
        // Arrange
        var product = new ValidProductFaker().Generate();
        var repository = new ProductRepository();

        // Act
        var foundProducts = repository.FindBy(product.Category, product.CreationDate, product.WarehouseId);

        // Assert
        foundProducts.Should().NotBeNull();
        foundProducts.Should().BeEmpty();
    }
}