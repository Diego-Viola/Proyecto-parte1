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
public class UpdateProductHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly UpdateProductHandler _handler;

    public UpdateProductHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _handler = new UpdateProductHandler(_productRepositoryMock.Object, _categoryRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateProduct_WhenProductAndCategoryExist_AndCategoryUnchanged()
    {
        var productId = 1L;
        var input = new UpdateProductInput
        {
            Name = "UpdatedName",
            Description = "UpdatedDescription",
            Price = 200,
            Stock = 10,
            CategoryId = 5
        };
        var product = new Product
        {
            Id = productId,
            Name = "OldName",
            Description = "OldDescription",
            Price = 100,
            Stock = 5,
            CategoryId = 5
        };
        var request = new UpdateProduct(productId, input);

        _productRepositoryMock.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);

        var act = async () => await _handler.Handle(request, default);

        await act.Should().NotThrowAsync();
        product.Name.Should().Be(input.Name);
        product.Description.Should().Be(input.Description);
        product.Price.Should().Be(input.Price);
        product.Stock.Should().Be(input.Stock);
        product.CategoryId.Should().Be(input.CategoryId);
        _productRepositoryMock.Verify(r => r.UpdateAsync(product), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldUpdateProductAndCategory_WhenCategoryChangedAndExists()
    {
        var productId = 2L;
        var input = new UpdateProductInput
        {
            Name = "NewName",
            Description = "NewDesc",
            Price = 300,
            Stock = 20,
            CategoryId = 99
        };
        var product = new Product
        {
            Id = productId,
            Name = "OldName",
            Description = "OldDesc",
            Price = 150,
            Stock = 8,
            CategoryId = 5
        };
        var category = new Category { Id = 99, Name = "NewCategory" };
        var request = new UpdateProduct(productId, input);

        _productRepositoryMock.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);
        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync(category);

        var act = async () => await _handler.Handle(request, default);

        await act.Should().NotThrowAsync();
        product.CategoryId.Should().Be(99);
        product.Name.Should().Be(input.Name);
        _productRepositoryMock.Verify(r => r.UpdateAsync(product), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowBusinessException_WhenPriceIsZeroOrNegative()
    {
        var productId = 10L;
        var input = new UpdateProductInput
        {
            Name = "TestProduct",
            Description = "TestDescription",
            Price = 0,
            Stock = 5,
            CategoryId = 1
        };
        var request = new UpdateProduct(productId, input);

        var act = async () => await _handler.Handle(request, default);

        await act.Should()
            .ThrowAsync<BusinessException>()
            .WithMessage("*El precio del producto debe ser mayor a cero*")
            .Where(e => e.Code == "API-UP-01");
        _productRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowBusinessException_WhenStockIsNegative()
    {
        var productId = 11L;
        var input = new UpdateProductInput
        {
            Name = "TestProduct",
            Description = "TestDescription",
            Price = 100,
            Stock = -1,
            CategoryId = 1
        };
        var request = new UpdateProduct(productId, input);

        var act = async () => await _handler.Handle(request, default);

        await act.Should()
            .ThrowAsync<BusinessException>()
            .WithMessage("*El stock del producto no puede ser negativo*")
            .Where(e => e.Code == "API-UP-02");
        _productRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenProductDoesNotExist()
    {
        var productId = 3L;
        var input = new UpdateProductInput
        {
            Name = "Any",
            Description = "Any",
            Price = 1,
            Stock = 1,
            CategoryId = 1
        };
        var request = new UpdateProduct(productId, input);
        _productRepositoryMock.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync((Product?)null);

        var act = async () => await _handler.Handle(request, default);

        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("*El producto no fue encontrado*")
            .Where(e => e.Code == "API-UP-03");
        _productRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequestException_WhenCategoryChangedAndDoesNotExist()
    {
        var productId = 4L;
        var input = new UpdateProductInput
        {
            Name = "Any",
            Description = "Any",
            Price = 1,
            Stock = 1,
            CategoryId = 100
        };
        var product = new Product
        {
            Id = productId,
            Name = "Old",
            Description = "Old",
            Price = 1,
            Stock = 1,
            CategoryId = 1
        };
        var request = new UpdateProduct(productId, input);

        _productRepositoryMock.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);
        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(100)).ReturnsAsync((Category?)null);

        Func<Task> act = async () => await _handler.Handle(request, default);

        var exception = await Assert.ThrowsAsync<BadRequestException>(act);
        exception.Message.Should().Contain("La categoría del producto no fue encontrada");
        exception.Code.Should().Be("API-UP-04");
        _productRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryWithCorrectParameters()
    {
        var productId = 5L;
        var input = new UpdateProductInput
        {
            Name = "RepoTest",
            Description = "RepoDesc",
            Price = 50,
            Stock = 2,
            CategoryId = 7
        };
        var product = new Product
        {
            Id = productId,
            Name = "Old",
            Description = "Old",
            Price = 1,
            Stock = 1,
            CategoryId = 7
        };
        var request = new UpdateProduct(productId, input);

        _productRepositoryMock.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);

        await _handler.Handle(request, default);

        _productRepositoryMock.Verify(r => r.GetByIdAsync(productId), Times.Once);
        _productRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Product>(p =>
            p.Id == productId &&
            p.Name == input.Name &&
            p.Description == input.Description &&
            p.Price == input.Price &&
            p.Stock == input.Stock &&
            p.CategoryId == input.CategoryId
        )), Times.Once);
    }
}
