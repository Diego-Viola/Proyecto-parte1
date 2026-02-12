using FluentAssertions;
using Moq;
using System.ComponentModel;
using WebApiTest.Application.Exceptions;
using WebApiTest.Application.Features.Categories.Queries;
using WebApiTest.Application.Interfaces.IRepositories;
using WebApiTest.Domain.Models;

namespace WebApiTest.Application.Test.Features.Categories;

[Category("Unit")]
public class GetCategoryDetailHandlerTests
{
    private Mock<ICategoryRepository> _categoryRepositoryMock;
    private GetCategoryDetailHandler _handler;

    public GetCategoryDetailHandlerTests()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();

        _handler = new GetCategoryDetailHandler(_categoryRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsCategoryOutput_WhenCategoryExists()
    {
        var request = new GetCategoryDetail(10);
        var category = new Category { Id = 10, Name = "TestCategory" };

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(category);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(10);
        result.Name.Should().Be("TestCategory");
    }

    [Fact]
    public async Task Handle_ThrowsNotFoundException_WhenCategoryDoesNotExist()
    {
        var request = new GetCategoryDetail(99);
        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Category?)null);

        Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryWithCorrectParameter()
    {
        var categoryId = 77L;
        var request = new GetCategoryDetail(categoryId);
        var category = new Category { Id = categoryId, Name = "RepoTestCategory" };

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync(category);

        await _handler.Handle(request, CancellationToken.None);

        _categoryRepositoryMock.Verify(r => r.GetByIdAsync(categoryId), Times.Once);
    }
}
