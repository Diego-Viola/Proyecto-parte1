using FluentAssertions;
using Moq;
using System.ComponentModel;
using WebApiTest.Application.Exceptions;
using WebApiTest.Application.Features.Products.Commands;
using WebApiTest.Application.Interfaces.IRepositories;
using WebApiTest.Domain.Models;

namespace WebApiTest.Application.Test.Features.Products;

[Category("Unit")]
public class DeleteProductHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly DeleteProductHandler _handler;

    public DeleteProductHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _handler = new DeleteProductHandler(_productRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeleteProduct_WhenProductExists()
    {
        var productId = 42L;
        var product = new Product { Id = productId, Name = "TestProduct" };
        var request = new DeleteProduct(productId);

        _productRepositoryMock.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);
        _productRepositoryMock.Setup(r => r.DeleteAsync(productId));

        var act = async () => await _handler.Handle(request, default);

        await act.Should().NotThrowAsync();
        _productRepositoryMock.Verify(r => r.DeleteAsync(productId), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenProductDoesNotExist()
    {
        var productId = 99L;
        var request = new DeleteProduct(productId);
        _productRepositoryMock.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync((Product?)null);

        var act = async () => await _handler.Handle(request, default);

        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("*El producto no fue encontrado*")
            .Where(e => e.Code == "API-DP-01");
        _productRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<long>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoriesWithCorrectParameters()
    {
        var productId = 77L;
        var product = new Product { Id = productId, Name = "RepoTestProduct" };
        var request = new DeleteProduct(productId);

        _productRepositoryMock.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);
        _productRepositoryMock.Setup(r => r.DeleteAsync(productId));

        await _handler.Handle(request, default);

        _productRepositoryMock.Verify(r => r.GetByIdAsync(productId), Times.Once);
        _productRepositoryMock.Verify(r => r.DeleteAsync(productId), Times.Once);
    }
}
