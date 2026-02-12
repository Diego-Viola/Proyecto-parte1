using FluentAssertions;
using Moq;
using System.ComponentModel;
using WebApiTest.Application.DTOs.Generics;
using WebApiTest.Application.Exceptions;
using WebApiTest.Application.Interfaces.IRepositories;
using WebApiTest.Application.Services;
using WebApiTest.Domain.Exceptions;
using WebApiTest.Domain.Models;

namespace WebApiTest.Application.Test.Services;

[Category("Unit")]
public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly CategoryService _categoryService;

    public CategoryServiceTests()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _categoryService = new CategoryService(_categoryRepositoryMock.Object);
    }

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_ShouldCreateCategory_WhenNameIsUnique()
    {
        var name = "UniqueName";
        _categoryRepositoryMock.Setup(r => r.GetByNameAsync(name)).ReturnsAsync((Category?)null);
        _categoryRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Category>()))
            .ReturnsAsync((Category c) => { c.Id = 123; return c; });

        var result = await _categoryService.CreateAsync(name);

        result.Should().NotBeNull();
        result.Id.Should().Be(123);
        result.Name.Should().Be(name);
        _categoryRepositoryMock.Verify(r => r.AddAsync(It.Is<Category>(c => c.Name == name)), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowBusinessException_WhenNameAlreadyExists()
    {
        var name = "DuplicateName";
        _categoryRepositoryMock.Setup(r => r.GetByNameAsync(name))
            .ReturnsAsync(new Category { Id = 1, Name = name });

        var act = async () => await _categoryService.CreateAsync(name);

        await act.Should()
            .ThrowAsync<BusinessException>()
            .WithMessage("*Ya existe una categoria con el mismo nombre*")
            .Where(e => e.Code == "ATI-CC-01");
    }

    #endregion

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ReturnsMappedCategoryOutputs_WhenCategoriesExist()
    {
        var count = 2;
        var page = 1;
        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Cat1" },
            new Category { Id = 2, Name = "Cat2" }
        };

        var paginationResult = new PaginationResult<Category>
        {
            Items = categories,
            Total = categories.Count
        };

        _categoryRepositoryMock.Setup(r => r.GetAllAsync(count, page)).ReturnsAsync(paginationResult);

        var result = await _categoryService.GetAllAsync(count, page);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(categories.Count);

        var resultList = result.Items.ToList();
        for (int i = 0; i < resultList.Count; i++)
        {
            categories[i].Id.Should().Be(resultList[i].Id);
            categories[i].Name.Should().Be(resultList[i].Name);
        }
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmpty_WhenNoCategoriesExist()
    {
        var count = 10;
        var page = 1;

        var paginationResult = new PaginationResult<Category>
        {
            Items = new List<Category>(),
            Total = 0
        };

        _categoryRepositoryMock.Setup(r => r.GetAllAsync(count, page)).ReturnsAsync(paginationResult);

        var result = await _categoryService.GetAllAsync(count, page);

        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.Total.Should().Be(0);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCategory_WhenCategoryExists()
    {
        var categoryId = 1L;
        var category = new Category { Id = categoryId, Name = "TestCategory" };

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync(category);

        var result = await _categoryService.GetByIdAsync(categoryId);

        result.Should().NotBeNull();
        result.Id.Should().Be(categoryId);
        result.Name.Should().Be(category.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowNotFoundException_WhenCategoryDoesNotExist()
    {
        var categoryId = 999L;
        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync((Category?)null);

        var act = async () => await _categoryService.GetByIdAsync(categoryId);

        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("*La categoria solicitada no existe*");
    }

    #endregion
}