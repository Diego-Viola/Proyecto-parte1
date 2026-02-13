using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;
using Products.Api.Controllers;
using Products.Api.Application.Interfaces.IServices;
using Products.Api.Application.DTOs.Outputs.Categories;
using Products.Api.Application.DTOs.Generics;
using Products.Api.Controllers.Requests;
using Products.Api.Application.Exceptions;
using Products.Api.Domain.Exceptions;

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
        var categories = new PaginationResult<CategoryOutput>
        {
            Items = new List<CategoryOutput>
            {
                new() { Id = 1, Name = "Electronics" },
                new() { Id = 2, Name = "Home" }
            },
            Total = 2
        };

        _categoryServiceMock
            .Setup(x => x.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(categories);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCategories = okResult.Value.Should().BeOfType<PaginationResult<CategoryOutput>>().Subject;
        returnedCategories.Items.Should().HaveCount(2);
        returnedCategories.Total.Should().Be(2);
    }

    [Fact]
    public async Task GetAll_WhenNoCategories_ReturnsNoContent()
    {
        // Arrange
        var emptyCategories = new PaginationResult<CategoryOutput>
        {
            Items = new List<CategoryOutput>(),
            Total = 0
        };

        _categoryServiceMock
            .Setup(x => x.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(emptyCategories);

        // Act
        var result = await _controller.GetAll();

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task GetAll_WithPagination_PassesCorrectParameters()
    {
        // Arrange
        int capturedCount = 0;
        int capturedPage = 0;

        _categoryServiceMock
            .Setup(x => x.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .Callback<int, int>((count, page) =>
            {
                capturedCount = count;
                capturedPage = page;
            })
            .ReturnsAsync(new PaginationResult<CategoryOutput> { Items = new List<CategoryOutput>(), Total = 0 });

        // Act
        await _controller.GetAll(count: 50, page: 3);

        // Assert
        capturedCount.Should().Be(50);
        capturedPage.Should().Be(3);
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_WhenCategoryExists_ReturnsOkWithCategory()
    {
        // Arrange
        var category = new CategoryOutput { Id = 1, Name = "Electronics" };
        _categoryServiceMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(category);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedCategory = okResult.Value.Should().BeOfType<CategoryOutput>().Subject;
        returnedCategory.Id.Should().Be(1);
        returnedCategory.Name.Should().Be("Electronics");
    }

    [Fact]
    public async Task GetById_WhenCategoryNotExists_ServiceThrowsNotFoundException()
    {
        // Arrange
        _categoryServiceMock
            .Setup(x => x.GetByIdAsync(999))
            .ThrowsAsync(new NotFoundException("Category not found", "NOT_FOUND"));

        // Act
        Func<Task> act = async () => await _controller.GetById(999);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Category not found");
    }

    #endregion

    #region Create Tests

    [Fact]
    public async Task Create_WithValidRequest_ReturnsCreatedWithCategory()
    {
        // Arrange
        var request = new CreateCategoryRequest { Name = "New Category" };
        var createdCategory = new CategoryOutput { Id = 10, Name = "New Category" };

        _categoryServiceMock
            .Setup(x => x.CreateAsync(request.Name))
            .ReturnsAsync(createdCategory);

        // Act
        var result = await _controller.Create(request);

        // Assert
        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(201);
        
        var returnedCategory = objectResult.Value.Should().BeOfType<CategoryOutput>().Subject;
        returnedCategory.Id.Should().Be(10);
        returnedCategory.Name.Should().Be("New Category");
    }

    [Fact]
    public async Task Create_WithDuplicateName_ServiceThrowsBusinessException()
    {
        // Arrange
        var request = new CreateCategoryRequest { Name = "Existing Category" };

        _categoryServiceMock
            .Setup(x => x.CreateAsync(request.Name))
            .ThrowsAsync(new BusinessException("Category already exists", "DUPLICATE_CATEGORY"));

        // Act
        Func<Task> act = async () => await _controller.Create(request);

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("Category already exists");
    }

    #endregion
}
