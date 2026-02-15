using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;
using Products.Api.Controllers;
using Products.Api.Application.Interfaces.IServices;
using Products.Api.Application.DTOs.Outputs.Products;
using Products.Api.Application.DTOs.Outputs.Categories;
using Products.Api.Application.DTOs.Inputs.Products;
using Products.Api.Application.DTOs.Outputs.Generics;
using Products.Api.Application.DTOs.Outputs.ProductDetail;
using Products.Api.Application.Exceptions;

namespace Products.Api.Test.Unit.Controllers;

/// <summary>
/// Tests unitarios para ProductsController.
/// </summary>
public class ProductsControllerTests
{
    private readonly Mock<IProductService> _productServiceMock;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _productServiceMock = new Mock<IProductService>();
        _controller = new ProductsController(_productServiceMock.Object);
    }

    #region GetAll Tests

    [Fact]
    public async Task GetAll_WhenProductsExist_ReturnsOkWithProducts()
    {
        // Arrange
        var products = new PaginationResult<ProductOutput>
        {
            Items = new List<ProductOutput>
            {
                new() { Id = 1, Name = "Product 1", Price = 100, CategoryId = 1 },
                new() { Id = 2, Name = "Product 2", Price = 200, CategoryId = 1 }
            },
            Total = 2
        };

        _productServiceMock
            .Setup(x => x.GetAllAsync(It.IsAny<GetProductsInput>()))
            .ReturnsAsync(products);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProducts = okResult.Value.Should().BeOfType<PaginationResult<ProductOutput>>().Subject;
        returnedProducts.Items.Should().HaveCount(2);
        returnedProducts.Total.Should().Be(2);
    }

    [Fact]
    public async Task GetAll_WhenNoProducts_ReturnsNoContent()
    {
        // Arrange
        var emptyProducts = new PaginationResult<ProductOutput>
        {
            Items = new List<ProductOutput>(),
            Total = 0
        };

        _productServiceMock
            .Setup(x => x.GetAllAsync(It.IsAny<GetProductsInput>()))
            .ReturnsAsync(emptyProducts);

        // Act
        var result = await _controller.GetAll();

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task GetAll_WithPagination_PassesCorrectParameters()
    {
        // Arrange
        GetProductsInput capturedInput = null!;
        _productServiceMock
            .Setup(x => x.GetAllAsync(It.IsAny<GetProductsInput>()))
            .Callback<GetProductsInput>(input => capturedInput = input)
            .ReturnsAsync(new PaginationResult<ProductOutput> { Items = new List<ProductOutput>(), Total = 0 });

        // Act
        await _controller.GetAll(count: 50, page: 3, name: "test", categoryId: 5);

        // Assert
        capturedInput.Should().NotBeNull();
        capturedInput.Count.Should().Be(50);
        capturedInput.Page.Should().Be(3);
        capturedInput.Name.Should().Be("test");
        capturedInput.CategoryId.Should().Be(5);
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_WhenProductExists_ReturnsOkWithProduct()
    {
        // Arrange
        var product = CreateTestProductDetail(1);
        _productServiceMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(product);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProduct = okResult.Value.Should().BeOfType<ProductDetailOutput>().Subject;
        returnedProduct.Id.Should().Be(1);
        returnedProduct.Name.Should().Be("Test Product 1");
    }

    [Fact]
    public async Task GetById_WhenProductNotExists_ServiceThrowsNotFoundException()
    {
        // Arrange
        _productServiceMock
            .Setup(x => x.GetByIdAsync(999))
            .ThrowsAsync(new NotFoundException("Product not found", "NOT_FOUND"));

        // Act
        Func<Task> act = async () => await _controller.GetById(999);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Product not found");
    }

    #endregion

    #region GetDetailById Tests

    [Fact]
    public async Task GetDetailById_WhenProductExists_ReturnsEnrichedProduct()
    {
        // Arrange
        var product = CreateTestProductDetail(1);
        _productServiceMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(product);

        // Act
        var result = await _controller.GetDetailById(1);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var enrichedProduct = okResult.Value.Should().BeOfType<ProductDetailEnrichedOutput>().Subject;
        
        enrichedProduct.Id.Should().Be(1);
        enrichedProduct.Name.Should().Be("Test Product 1");
        enrichedProduct.Images.Should().NotBeEmpty();
        enrichedProduct.Seller.Should().NotBeNull();
        enrichedProduct.Shipping.Should().NotBeNull();
        enrichedProduct.Rating.Should().NotBeNull();
    }

    [Fact]
    public async Task GetDetailById_WhenProductNotExists_ServiceThrowsNotFoundException()
    {
        // Arrange
        _productServiceMock
            .Setup(x => x.GetByIdAsync(999))
            .ThrowsAsync(new NotFoundException("Product not found", "NOT_FOUND"));

        // Act
        Func<Task> act = async () => await _controller.GetDetailById(999);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion

    #region GetRelatedProducts Tests

    [Fact]
    public async Task GetRelatedProducts_WhenProductExists_ReturnsRelatedProducts()
    {
        // Arrange
        var product = CreateTestProductDetail(1);
        _productServiceMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(product);

        // Act
        var result = await _controller.GetRelatedProducts(1, limit: 6);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var relatedProducts = okResult.Value.Should().BeAssignableTo<List<ProductSummaryOutput>>().Subject;
        
        relatedProducts.Should().HaveCount(6);
        relatedProducts.Should().AllSatisfy(p =>
        {
            p.Id.Should().NotBe(1); // No debe incluir el producto original
            p.Name.Should().NotBeNullOrEmpty();
            p.Price.Should().BeGreaterThan(0);
        });
    }

    [Fact]
    public async Task GetRelatedProducts_WhenProductNotExists_ServiceThrowsNotFoundException()
    {
        // Arrange
        _productServiceMock
            .Setup(x => x.GetByIdAsync(999))
            .ThrowsAsync(new NotFoundException("Product not found", "NOT_FOUND"));

        // Act
        Func<Task> act = async () => await _controller.GetRelatedProducts(999);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_WhenProductExists_ReturnsNoContent()
    {
        // Arrange
        _productServiceMock
            .Setup(x => x.DeleteAsync(1))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _productServiceMock.Verify(x => x.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task Delete_WhenProductNotExists_ServiceThrowsNotFoundException()
    {
        // Arrange
        _productServiceMock
            .Setup(x => x.DeleteAsync(999))
            .ThrowsAsync(new NotFoundException("Product not found", "NOT_FOUND"));

        // Act
        Func<Task> act = async () => await _controller.Delete(999);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion

    #region Helper Methods

    private static ProductDetailOutput CreateTestProductDetail(long id)
    {
        return new ProductDetailOutput
        {
            Id = id,
            Name = $"Test Product {id}",
            Description = $"Description for product {id}",
            Price = 100 * id,
            Stock = 10,
            Category = new CategoryOutput
            {
                Id = 1,
                Name = "Electronics"
            }
        };
    }

    #endregion
}
