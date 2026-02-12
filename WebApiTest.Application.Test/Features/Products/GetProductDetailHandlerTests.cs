using FluentAssertions;
using Moq;
using System.ComponentModel;
using WebApiTest.Application.Exceptions;
using WebApiTest.Application.Features.Products.Queries;
using WebApiTest.Application.Interfaces.IRepositories;
using WebApiTest.Domain.Models;

namespace WebApiTest.Application.Test.Features.Products;

[Category("Unit")]
public class GetProductDetailHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly GetProductDetailHandler _handler;

    public GetProductDetailHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _handler = new GetProductDetailHandler(_productRepositoryMock.Object, _categoryRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnProductDetail_WhenProductAndCategoryExist()
    {
        var productId = 1L;
        var categoryId = 10L;
        var product = new Product
        {
            Id = productId,
            Name = "TestProduct",
            Description = "TestDescription",
            Price = 100,
            Stock = 5,
            CategoryId = categoryId
        };
        var category = new Category
        {
            Id = categoryId,
            Name = "TestCategory"
        };

        _productRepositoryMock.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);
        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync(category);

        var request = new GetProductDetail(productId);

        var result = await _handler.Handle(request, default);

        result.Should().NotBeNull();
        result.Id.Should().Be(product.Id);
        result.Name.Should().Be(product.Name);
        result.Description.Should().Be(product.Description);
        result.Price.Should().Be(product.Price);
        result.Stock.Should().Be(product.Stock);
        result.Category.Should().NotBeNull();
        result.Category.Id.Should().Be(category.Id);
        result.Category.Name.Should().Be(category.Name);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenProductDoesNotExist()
    {
        var productId = 2L;
        _productRepositoryMock.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync((Product?)null);
        var request = new GetProductDetail(productId);

        var act = async () => await _handler.Handle(request, default);

        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("*El producto no fue encontrado con el id especificado*")
            .Where(e => e.Code == "API-GPD-01");
    }

    [Fact]
    public async Task Handle_ShouldThrowDataIntegrationException_WhenCategoryDoesNotExist()
    {
        var productId = 3L;
        var categoryId = 20L;
        var product = new Product
        {
            Id = productId,
            Name = "TestProduct2",
            Description = "TestDescription2",
            Price = 200,
            Stock = 10,
            CategoryId = categoryId
        };

        _productRepositoryMock.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);
        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync((Category?)null);
        var request = new GetProductDetail(productId);

        var act = async () => await _handler.Handle(request, default);

        await act.Should()
            .ThrowAsync<DataIntegrationException>()
            .WithMessage("*La categoría del producto registrado no fue encontrada*");
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoriesWithCorrectParameters()
    {
        var productId = 5L;
        var categoryId = 15L;
        var product = new Product
        {
            Id = productId,
            Name = "RepoTestProduct",
            Description = "RepoTestDescription",
            Price = 50,
            Stock = 2,
            CategoryId = categoryId
        };
        var category = new Category
        {
            Id = categoryId,
            Name = "RepoTestCategory"
        };

        _productRepositoryMock.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);
        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync(category);

        var request = new GetProductDetail(productId);

        await _handler.Handle(request, default);

        _productRepositoryMock.Verify(r => r.GetByIdAsync(productId), Times.Once);
        _categoryRepositoryMock.Verify(r => r.GetByIdAsync(categoryId), Times.Once);
    }
}
