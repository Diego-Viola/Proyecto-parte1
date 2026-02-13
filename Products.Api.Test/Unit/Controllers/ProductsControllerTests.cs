using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;
using Products.Api.Controllers;
using Products.Api.Application.Interfaces.IServices;
using Products.Api.Application.DTOs.Outputs.Products;
using Products.Api.Application.DTOs.Outputs.Categories;
using Products.Api.Application.DTOs.Generics;

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
        // TODO: Implementar test

        // Act

        // Assert
        Assert.True(true); // Placeholder
    }

    [Fact]
    public async Task GetAll_WhenNoProducts_ReturnsNoContent()
    {
        // Arrange
        // TODO: Implementar test

        // Act

        // Assert
        Assert.True(true); // Placeholder
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_WhenProductExists_ReturnsOkWithProduct()
    {
        // Arrange
        // TODO: Implementar test

        // Act

        // Assert
        Assert.True(true); // Placeholder
    }

    #endregion

    #region GetDetailById Tests

    [Fact]
    public async Task GetDetailById_WhenProductExists_ReturnsEnrichedProduct()
    {
        // Arrange
        // TODO: Implementar test

        // Act

        // Assert
        Assert.True(true); // Placeholder
    }

    #endregion
}
