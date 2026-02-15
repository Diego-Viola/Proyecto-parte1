using FluentAssertions;
using System.ComponentModel;
using System.Net;
using System.Net.Http.Json;
using Products.Api.Integration.Test.Support;
using Products.Api.Application.DTOs.Inputs.Category;
using Products.Api.Application.DTOs.Outputs.Categories;
using Products.Api.Application.DTOs.Outputs.Generics;

namespace Products.Api.Integration.Test.Controllers;

[Category("Integration")]
public class CategoryControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CategoryControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithCategories()
    {
        var response = await _client.GetAsync("/api/v1/categories?count=10&page=1");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var paginationResult = await response.Content.ReadFromJsonAsync<PaginationResult<CategoryOutput>>();
            paginationResult.Should().NotBeNull();
            paginationResult!.Items.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenCategoryDoesNotExist()
    {
        var response = await _client.GetAsync("/api/v1/categories/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_ReturnsCreated_AndCanBeRetrieved()
    {
        var request = new CreateCategoryInput() { Name = $"TestCat_{Guid.NewGuid()}" };
        var postResponse = await _client.PostAsJsonAsync("/api/v1/categories", request);
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await postResponse.Content.ReadFromJsonAsync<CategoryOutput>();
        created.Should().NotBeNull();
        created!.Name.Should().Be(request.Name);

        var getResponse = await _client.GetAsync($"/api/v1/categories/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var fetched = await getResponse.Content.ReadFromJsonAsync<CategoryOutput>();
        fetched.Should().NotBeNull();
        fetched!.Id.Should().Be(created.Id);
        fetched.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task GetById_ReturnsCategoryDetails_WhenCategoryExists()
    {
        var request = new CreateCategoryInput() { Name = $"TestCat_{Guid.NewGuid()}" };
        var postResponse = await _client.PostAsJsonAsync("/api/v1/categories", request);
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await postResponse.Content.ReadFromJsonAsync<CategoryOutput>();
        created.Should().NotBeNull();

        var getResponse = await _client.GetAsync($"/api/v1/categories/{created!.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var fetched = await getResponse.Content.ReadFromJsonAsync<CategoryOutput>();
        fetched.Should().NotBeNull();
        fetched!.Id.Should().Be(created.Id);
        fetched.Name.Should().Be(request.Name);
    }
}
