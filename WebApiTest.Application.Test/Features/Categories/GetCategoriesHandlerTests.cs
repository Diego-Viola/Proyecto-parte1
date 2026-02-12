using FluentAssertions;
using Moq;
using System.ComponentModel;
using WebApiTest.Application.DTOs.Generics;
using WebApiTest.Application.Features.Categories.Queries;
using WebApiTest.Application.Interfaces.IRepositories;
using WebApiTest.Domain.Models;

namespace WebApiTest.Application.Test.Features.Categories;

[Category("Unit")]
public class GetCategoriesHandlerTests
{
    private Mock<ICategoryRepository> _categoryRepositoryMock;
    private GetCategoriesHandler _handler;

    public GetCategoriesHandlerTests()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _handler = new GetCategoriesHandler(_categoryRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsMappedCategoryOutputs_WhenCategoriesExist()
    {
        var request = new GetCategories(2, 1);
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

        _categoryRepositoryMock.Setup(r => r.GetAllAsync(2, 1)).ReturnsAsync(paginationResult);

        var result = await _handler.Handle(request, CancellationToken.None);

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
    public async Task Handle_ReturnsEmpty_WhenNoCategoriesExist()
    {
        var request = new GetCategories(10, 1);

        var paginationResult = new PaginationResult<Category>
        {
            Items = new List<Category>(),
            Total = 0
        };

        _categoryRepositoryMock.Setup(r => r.GetAllAsync(10, 1)).ReturnsAsync(paginationResult);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.Total.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryWithCorrectParameters()
    {
        var request = new GetCategories(7, 3);

        var paginationResult = new PaginationResult<Category>
        {
            Items = new List<Category>(),
            Total = 0
        };

        _categoryRepositoryMock.Setup(r => r.GetAllAsync(7, 3)).ReturnsAsync(paginationResult);

        await _handler.Handle(request, CancellationToken.None);

        _categoryRepositoryMock.Verify(r => r.GetAllAsync(7, 3), Times.Once);
    }
}
