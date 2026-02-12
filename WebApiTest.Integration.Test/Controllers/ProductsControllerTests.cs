using FluentAssertions;
using System.ComponentModel;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WebApiTest.Application.DTOs.Generics;
using WebApiTest.Application.DTOs.Outputs.Products;
using WebApiTest.Common;
using WebApiTest.Controllers.Requests;
using WebApiTest.Integration.Test.Support;

namespace WebApiTest.Integration.Test.Controllers;

[Category("Integration")]
public class ProductsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _camelCaseOptions;

    public ProductsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _camelCaseOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    [Fact]
    public async Task GetAll_ReturnsOkOrNoContent()
    {
        var response = await _client.GetAsync("/api/v1/products?count=10&page=1");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var paginationResult = await response.Content.ReadFromJsonAsync<PaginationResult<ProductOutput>>(_camelCaseOptions);
            paginationResult.Should().NotBeNull();
            paginationResult!.Items.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenProductDoesNotExist()
    {
        var response = await _client.GetAsync("/api/v1/products/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(_camelCaseOptions);
        error.Should().NotBeNull();
        error!.Status.Should().Be((int)HttpStatusCode.NotFound);
        error.Title.Should().Be("Not Found");
        error.Type.Should().Be("https://yourdomain.com/errors/not-found");
    }

    [Fact]
    public async Task Create_ReturnsCreated_AndCanBeRetrieved()
    {
        var createRequest = new CreateProductRequest
        {
            Name = $"TestProduct_{Guid.NewGuid()}",
            Description = "Test Description",
            Price = 10.5m,
            Stock = 5,
            CategoryId = 1
        };

        var postResponse = await _client.PostAsync(
            "/api/v1/products",
            JsonContent.Create(createRequest, options: _camelCaseOptions)
        );
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await postResponse.Content.ReadFromJsonAsync<ProductDetailOutput>(_camelCaseOptions);
        created.Should().NotBeNull();
        created!.Name.Should().Be(createRequest.Name);

        var getResponse = await _client.GetAsync($"/api/v1/products/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var fetched = await getResponse.Content.ReadFromJsonAsync<ProductDetailOutput>(_camelCaseOptions);
        fetched.Should().NotBeNull();
        fetched!.Id.Should().Be(created.Id);
        fetched.Name.Should().Be(createRequest.Name);
    }

    [Fact]
    public async Task GetById_ReturnsProductDetails_WhenProductExists()
    {
        var createRequest = new CreateProductRequest
        {
            Name = $"TestProduct_{Guid.NewGuid()}",
            Description = "Test Description",
            Price = 10.5m,
            Stock = 5,
            CategoryId = 1
        };

        var postResponse = await _client.PostAsync(
            "/api/v1/products",
            JsonContent.Create(createRequest, options: _camelCaseOptions)
        );
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await postResponse.Content.ReadFromJsonAsync<ProductDetailOutput>(_camelCaseOptions);
        created.Should().NotBeNull();

        var getResponse = await _client.GetAsync($"/api/v1/products/{created!.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var fetched = await getResponse.Content.ReadFromJsonAsync<ProductDetailOutput>(_camelCaseOptions);
        fetched.Should().NotBeNull();
        fetched!.Id.Should().Be(created.Id);
        fetched.Name.Should().Be(createRequest.Name);
    }

    [Fact]
    public async Task Update_ReturnsNoContent()
    {
        var createRequest = new CreateProductRequest
        {
            Name = $"TestProduct_{Guid.NewGuid()}",
            Description = "Test Description",
            Price = 10.5m,
            Stock = 5,
            CategoryId = 1
        };

        var postResponse = await _client.PostAsync(
            "/api/v1/products",
            JsonContent.Create(createRequest, options: _camelCaseOptions)
        );
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await postResponse.Content.ReadFromJsonAsync<ProductDetailOutput>(_camelCaseOptions);
        created.Should().NotBeNull();

        var updateRequest = new UpdateProductRequest
        {
            Name = "Updated Name",
            Description = "Updated Description",
            Price = 20.0m,
            Stock = 10,
            CategoryId = 1
        };

        var putResponse = await _client.PutAsync(
            $"/api/v1/products/{created!.Id}",
            JsonContent.Create(updateRequest, options: _camelCaseOptions)
        );
        putResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/v1/products/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var fetched = await getResponse.Content.ReadFromJsonAsync<ProductDetailOutput>(_camelCaseOptions);
        fetched.Should().NotBeNull();
        fetched!.Name.Should().Be(updateRequest.Name);
        fetched.Description.Should().Be(updateRequest.Description);
        fetched.Price.Should().Be(updateRequest.Price);
        fetched.Stock.Should().Be(updateRequest.Stock);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var createRequest = new CreateProductRequest
        {
            Name = $"TestProduct_{Guid.NewGuid()}",
            Description = "Test Description",
            Price = 10.5m,
            Stock = 5,
            CategoryId = 1
        };

        var postResponse = await _client.PostAsync(
            "/api/v1/products",
            JsonContent.Create(createRequest, options: _camelCaseOptions)
        );
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await postResponse.Content.ReadFromJsonAsync<ProductDetailOutput>(_camelCaseOptions);
        created.Should().NotBeNull();

        var deleteResponse = await _client.DeleteAsync($"/api/v1/products/{created!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/v1/products/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}