using FluentAssertions;
using Moq;
using System.ComponentModel;
using WebApiTest.Application.DTOs.Inputs.Products;
using WebApiTest.Application.Exceptions;
using WebApiTest.Application.Interfaces.IRepositories;
using WebApiTest.Application.Services;
using WebApiTest.Domain.Exceptions;
using WebApiTest.Domain.Models;

namespace WebApiTest.Application.Test.Services;

[Category("Unit")]
public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _productService = new ProductService(_productRepositoryMock.Object, _categoryRepositoryMock.Object);
    }

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_ShouldCreateProduct_WhenCategoryExists()
    {
        var input = new CreateProductInput
        {
            Name = "TestProduct",
            Description = "TestDescription",
            Price = 100,
            Stock = 5,
            CategoryId = 10
        };
        var category = new Category { Id = 10, Name = "TestCategory" };
        var product = new Product
        {
            Id = 123,
            Name = input.Name,
            Description = input.Description,
            Price = input.Price,
            Stock = input.Stock,
            CategoryId = input.CategoryId
        };

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(input.CategoryId)).ReturnsAsync(category);
        _productRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Product>())).ReturnsAsync(product);

        var result = await _productService.CreateAsync(input);

        result.Should().NotBeNull();
        result.Id.Should().Be(123);
        result.Name.Should().Be(input.Name);
        result.Description.Should().Be(input.Description);
        result.Price.Should().Be(input.Price);
        result.Stock.Should().Be(input.Stock);
        result.Category.Should().NotBeNull();
        result.Category.Id.Should().Be(category.Id);
        result.Category.Name.Should().Be(category.Name);
        _productRepositoryMock.Verify(r => r.AddAsync(It.Is<Product>(p =>
            p.Name == input.Name &&
            p.Description == input.Description &&
            p.Price == input.Price &&
            p.Stock == input.Stock &&
            p.CategoryId == input.CategoryId)), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowBusinessException_WhenPriceIsZeroOrNegative()
    {
        var input = new CreateProductInput
        {
            Name = "TestProduct",
            Description = "TestDescription",
            Price = 0,
            Stock = 5,
            CategoryId = 10
        };

        var act = async () => await _productService.CreateAsync(input);

        await act.Should()
            .ThrowAsync<BusinessException>()
            .WithMessage("*El precio del producto debe ser mayor a cero*")
            .Where(e => e.Code == "API-CP-01");
        _productRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowBusinessException_WhenStockIsNegative()
    {
        var input = new CreateProductInput
        {
            Name = "TestProduct",
            Description = "TestDescription",
            Price = 100,
            Stock = -1,
            CategoryId = 10
        };

        var act = async () => await _productService.CreateAsync(input);

        await act.Should()
            .ThrowAsync<BusinessException>()
            .WithMessage("*El stock del producto no puede ser negativo*")
            .Where(e => e.Code == "API-CP-02");
        _productRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowBadRequestException_WhenCategoryDoesNotExist()
    {
        var input = new CreateProductInput
        {
            Name = "TestProduct",
            Description = "TestDescription",
            Price = 100,
            Stock = 5,
            CategoryId = 99
        };
        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(input.CategoryId)).ReturnsAsync((Category?)null);

        var act = async () => await _productService.CreateAsync(input);

        await act.Should()
            .ThrowAsync<BadRequestException>()
            .WithMessage("*La categoría del producto no fue encontrada*")
            .Where(e => e.Code == "API-CP-03");
        _productRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Never);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProductDetail_WhenProductExists()
    {
        var productId = 1L;
        var product = new Product
        {
            Id = productId,
            Name = "TestProduct",
            Description = "TestDescription",
            Price = 100,
            Stock = 5,
            CategoryId = 10
        };
        var category = new Category { Id = 10, Name = "TestCategory" };

        _productRepositoryMock.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);
        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(product.CategoryId)).ReturnsAsync(category);

        var result = await _productService.GetByIdAsync(productId);

        result.Should().NotBeNull();
        result.Id.Should().Be(productId);
        result.Name.Should().Be(product.Name);
        result.Category.Id.Should().Be(category.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowNotFoundException_WhenProductDoesNotExist()
    {
        var productId = 999L;
        _productRepositoryMock.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync((Product?)null);

        var act = async () => await _productService.GetByIdAsync(productId);

        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("*El producto no fue encontrado*");
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ShouldUpdateProduct_WhenProductExists()
    {
        var productId = 1L;
        var input = new UpdateProductInput
        {
            Name = "UpdatedProduct",
            Description = "UpdatedDescription",
            Price = 200,
            Stock = 10,
            CategoryId = 10
        };
        var existingProduct = new Product
        {
            Id = productId,
            Name = "OldProduct",
            Description = "OldDescription",
            Price = 100,
            Stock = 5,
            CategoryId = 10
        };

        _productRepositoryMock.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(existingProduct);
        _productRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Product>())).ReturnsAsync(true);

        await _productService.UpdateAsync(productId, input);

        _productRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Product>(p =>
            p.Name == input.Name &&
            p.Description == input.Description &&
            p.Price == input.Price &&
            p.Stock == input.Stock)), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowNotFoundException_WhenProductDoesNotExist()
    {
        var productId = 999L;
        var input = new UpdateProductInput
        {
            Name = "UpdatedProduct",
            Description = "UpdatedDescription",
            Price = 200,
            Stock = 10,
            CategoryId = 10
        };
        _productRepositoryMock.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync((Product?)null);

        var act = async () => await _productService.UpdateAsync(productId, input);

        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("*El producto no fue encontrado*");
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_ShouldDeleteProduct_WhenProductExists()
    {
        var productId = 1L;
        var existingProduct = new Product { Id = productId, Name = "TestProduct" };

        _productRepositoryMock.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(existingProduct);
        _productRepositoryMock.Setup(r => r.DeleteAsync(productId)).ReturnsAsync(true);

        await _productService.DeleteAsync(productId);

        _productRepositoryMock.Verify(r => r.DeleteAsync(productId), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowNotFoundException_WhenProductDoesNotExist()
    {
        var productId = 999L;
        _productRepositoryMock.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync((Product?)null);

        var act = async () => await _productService.DeleteAsync(productId);

        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("*El producto no fue encontrado*");
    }

    #endregion
}
