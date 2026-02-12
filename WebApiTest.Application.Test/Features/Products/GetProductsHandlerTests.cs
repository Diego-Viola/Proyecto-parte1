using FluentAssertions;
using Moq;
using System.ComponentModel;
using WebApiTest.Application.DTOs.Generics;
using WebApiTest.Application.DTOs.Inputs.Products;
using WebApiTest.Application.Features.Products.Queries;
using WebApiTest.Application.Interfaces.IRepositories;
using WebApiTest.Domain.Models;

[Category("Unit")]
public class GetProductsHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly GetProductsHandler _handler;

    public GetProductsHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _handler = new GetProductsHandler(_productRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedProductOutputs_WhenProductsExist()
    {
        var input = new GetProductsInput
        {
            Page = 1,
            Count = 2,
            Name = "Test",
            CategoryId = 10
        };
        var request = new GetProducts(input);
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Prod1", Price = 100, CategoryId = 10 },
            new Product { Id = 2, Name = "Prod2", Price = 200, CategoryId = 10 }
        };

        var paginationResult = new PaginationResult<Product>
        {
            Items = products,
            Total = products.Count
        };

        _productRepositoryMock
            .Setup(r => r.GetAllAsync(1, 2, "Test", 10))
            .ReturnsAsync(paginationResult);

        var result = await _handler.Handle(request, default);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(products.Count);

        var resultList = result.Items.ToList();

        for (int i = 0; i < resultList.Count; i++)
        {
            products[i].Id.Should().Be(resultList[i].Id);
            products[i].Name.Should().Be(resultList[i].Name);
            products[i].Price.Should().Be(resultList[i].Price);
            products[i].CategoryId.Should().Be(resultList[i].CategoryId);
        }
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenNoProductsExist()
    {
        var input = new GetProductsInput
        {
            Page = 1,
            Count = 2,
            Name = "Test",
            CategoryId = 10
        };
        var request = new GetProducts(input);

        var paginationResult = new PaginationResult<Product>
        {
            Items = new List<Product>(),
            Total = 0
        };

        _productRepositoryMock
            .Setup(r => r.GetAllAsync(1, 2, "Test", 10))
            .ReturnsAsync(paginationResult);

        var result = await _handler.Handle(request, default);

        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.Total.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryWithCorrectParameters()
    {
        var input = new GetProductsInput
        {
            Page = 3,
            Count = 5,
            Name = "FilterName",
            CategoryId = 99
        };
        var request = new GetProducts(input);

        var paginationResult = new PaginationResult<Product>
        {
            Items = new List<Product>(),
            Total = 0
        };

        _productRepositoryMock
            .Setup(r => r.GetAllAsync(3, 5, "FilterName", 99))
            .ReturnsAsync(paginationResult);

        await _handler.Handle(request, default);

        _productRepositoryMock.Verify(r => r.GetAllAsync(3, 5, "FilterName", 99), Times.Once);
    }
}