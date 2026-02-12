using FluentAssertions;
using Moq;
using System.ComponentModel;
using WebApiTest.Application.DTOs.Inputs.Products;
using WebApiTest.Application.Exceptions;
using WebApiTest.Application.Features.Products.Commands;
using WebApiTest.Application.Interfaces.IRepositories;
using WebApiTest.Domain.Exceptions;
using WebApiTest.Domain.Models;

namespace WebApiTest.Application.Test.Features.Products;

[Category("Unit")]
public class CreateProductHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly CreateProductHandler _handler;

    public CreateProductHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _handler = new CreateProductHandler(_productRepositoryMock.Object, _categoryRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateProduct_WhenCategoryExists()
    {
        var input = new CreateProductInput
        {
            Name = "TestProduct",
            Description = "TestDescription",
            Price = 100,
            Stock = 5,
            CategoryId = 10
        };
        var request = new CreateProduct(input);
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

        var result = await _handler.Handle(request, default);

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
    public async Task Handle_ShouldThrowBusinessException_WhenPriceIsZeroOrNegative()
    {
        var input = new CreateProductInput
        {
            Name = "TestProduct",
            Description = "TestDescription",
            Price = 0,
            Stock = 5,
            CategoryId = 10
        };
        var request = new CreateProduct(input);

        var act = async () => await _handler.Handle(request, default);

        await act.Should()
            .ThrowAsync<BusinessException>()
            .WithMessage("*El precio del producto debe ser mayor a cero*")
            .Where(e => e.Code == "API-CP-01");
        _productRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowBusinessException_WhenStockIsNegative()
    {
        var input = new CreateProductInput
        {
            Name = "TestProduct",
            Description = "TestDescription",
            Price = 100,
            Stock = -1,
            CategoryId = 10
        };
        var request = new CreateProduct(input);

        var act = async () => await _handler.Handle(request, default);

        await act.Should()
            .ThrowAsync<BusinessException>()
            .WithMessage("*El stock del producto no puede ser negativo*")
            .Where(e => e.Code == "API-CP-02");
        _productRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequestException_WhenCategoryDoesNotExist()
    {
        var input = new CreateProductInput
        {
            Name = "TestProduct",
            Description = "TestDescription",
            Price = 100,
            Stock = 5,
            CategoryId = 99
        };
        var request = new CreateProduct(input);
        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(input.CategoryId)).ReturnsAsync((Category?)null);

        var act = async () => await _handler.Handle(request, default);

        await act.Should()
            .ThrowAsync<BadRequestException>()
            .WithMessage("*La categoría del producto no fue encontrada*")
            .Where(e => e.Code == "API-CP-03");
        _productRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoriesWithCorrectParameters()
    {
        var input = new CreateProductInput
        {
            Name = "RepoTestProduct",
            Description = "RepoTestDescription",
            Price = 123.45m,
            Stock = 7,
            CategoryId = 55
        };
        var request = new CreateProduct(input);
        var category = new Category { Id = 55, Name = "RepoTestCategory" };
        var product = new Product
        {
            Id = 999,
            Name = input.Name,
            Description = input.Description,
            Price = input.Price,
            Stock = input.Stock,
            CategoryId = input.CategoryId
        };

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(input.CategoryId)).ReturnsAsync(category);
        _productRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Product>())).ReturnsAsync(product);

        await _handler.Handle(request, default);

        _categoryRepositoryMock.Verify(r => r.GetByIdAsync(input.CategoryId), Times.Once);
        _productRepositoryMock.Verify(r => r.AddAsync(It.Is<Product>(p =>
            p.Name == input.Name &&
            p.Description == input.Description &&
            p.Price == input.Price &&
            p.Stock == input.Stock &&
            p.CategoryId == input.CategoryId
        )), Times.Once);
    }
}
