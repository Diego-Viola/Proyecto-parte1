using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;
using Products.Api.Controllers;
using Products.Api.Application.Interfaces.IServices;
using Products.Api.Application.DTOs.Outputs.Categories;
using Products.Api.Application.DTOs.Generics;

namespace Products.Api.Test.Unit.Controllers;

/// <summary>
/// Tests unitarios para CategoriesController.
/// </summary>
public class CategoriesControllerTests
{
    private readonly Mock<ICategoryService> _categoryServiceMock;
    private readonly CategoriesController _controller;

    public CategoriesControllerTests()
    {
        _categoryServiceMock = new Mock<ICategoryService>();
        _controller = new CategoriesController(_categoryServiceMock.Object);
    }

    #region GetAll Tests

    [Fact]
    public async Task GetAll_WhenCategoriesExist_ReturnsOkWithCategories()
    {
        // Arrange
        // TODO: Implementar test

        // Act

        // Assert
        Assert.True(true); // Placeholder
    }

    [Fact]
    public async Task GetAll_WhenNoCategories_ReturnsNoContent()
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
    public async Task GetById_WhenCategoryExists_ReturnsOkWithCategory()
    {
        // Arrange
        // TODO: Implementar test

        // Act

        // Assert
        Assert.True(true); // Placeholder
    }

    #endregion

    #region Create Tests

    [Fact]
    public async Task Create_WithValidRequest_ReturnsCreatedWithCategory()
    {
        // Arrange
        // TODO: Implementar test

        // Act

        // Assert
        Assert.True(true); // Placeholder
    }

    #endregion
}
