using FluentAssertions;
using Moq;
using System.ComponentModel;
using WebApiTest.Application.Features.Categories.Commands;
using WebApiTest.Application.Interfaces.IRepositories;
using WebApiTest.Domain.Exceptions;
using WebApiTest.Domain.Models;

namespace WebApiTest.Application.Test.Features.Categories;

[Category("Unit")]
public class CreateCategoryHandlerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly CreateCategoryHandler _handler;

    public CreateCategoryHandlerTests()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _handler = new CreateCategoryHandler(_categoryRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateCategory_WhenNameIsUnique()
    {
        var request = new CreateCategory("UniqueName");
        _categoryRepositoryMock.Setup(r => r.GetByNameAsync("UniqueName")).ReturnsAsync((Category?)null);
        _categoryRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Category>()))
            .ReturnsAsync((Category c) => { c.Id = 123; return c; });

        var result = await _handler.Handle(request, default);

        result.Should().NotBeNull();
        result.Id.Should().Be(123);
        result.Name.Should().Be("UniqueName");
        _categoryRepositoryMock.Verify(r => r.AddAsync(It.Is<Category>(c => c.Name == "UniqueName")), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowBusinessException_WhenNameAlreadyExists()
    {
        var request = new CreateCategory("DuplicateName");
        _categoryRepositoryMock.Setup(r => r.GetByNameAsync("DuplicateName"))
            .ReturnsAsync(new Category { Id = 1, Name = "DuplicateName" });

        var act = async () => await _handler.Handle(request, default);

        await act.Should()
            .ThrowAsync<BusinessException>()
            .WithMessage("*Ya existe una categoria con el mismo nombre*")
            .Where(e => e.Code == "ATI-CC-01");
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoriesWithCorrectParameters()
    {
        var request = new CreateCategory("RepoTestCategory");
        _categoryRepositoryMock.Setup(r => r.GetByNameAsync("RepoTestCategory")).ReturnsAsync((Category?)null);
        _categoryRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Category>())).ReturnsAsync((Category c) => { c.Id = 456; return c; });

        await _handler.Handle(request, default);

        _categoryRepositoryMock.Verify(r => r.GetByNameAsync("RepoTestCategory"), Times.Once);
        _categoryRepositoryMock.Verify(r => r.AddAsync(It.Is<Category>(c => c.Name == "RepoTestCategory")), Times.Once);
    }
}
