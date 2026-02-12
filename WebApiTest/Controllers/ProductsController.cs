using Mapster;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using WebApiTest.Application.DTOs.Generics;
using WebApiTest.Application.DTOs.Inputs.Products;
using WebApiTest.Application.DTOs.Outputs.Products;
using WebApiTest.Application.Interfaces.IServices;
using WebApiTest.Controllers.Requests;

namespace WebApiTest.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Returns products list", Tags = new[] { "Products" })]
        [ProducesResponseType(typeof(PaginationResult<ProductOutput>), StatusCodes.Status200OK)]
        [Produces(MediaTypeNames.Application.Json, "application/problem+json")]
        public async Task<IActionResult> GetAll(
            [FromQuery, Required, Range(1, int.MaxValue)] int count,
            [FromQuery, Required, Range(1, int.MaxValue)] int page,
            [FromQuery] string? name,
            [FromQuery] long? categoryId = null
        )
        {
            var products = await _productService.GetAllAsync(new GetProductsInput()
            {
                Page = page,
                Count = count,
                Name = name,
                CategoryId = categoryId
            });

            return !products.Items.Any() ? NoContent() : Ok(products);
        }


        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Returns product detail", Tags = new[] { "Products" })]
        [ProducesResponseType(typeof(ProductDetailOutput), StatusCodes.Status200OK)]
        [Produces(MediaTypeNames.Application.Json, "application/problem+json")]
        public async Task<IActionResult> GetById(long id)
            => Ok(await _productService.GetByIdAsync(id));

        [HttpPost]
        [SwaggerOperation(Summary = "Create a new product", Tags = new[] { "Products" })]
        [ProducesResponseType(typeof(ProductDetailOutput), StatusCodes.Status201Created)]
        [Produces(MediaTypeNames.Application.Json, "application/problem+json")]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
            => StatusCode(201, await _productService.CreateAsync(request.Adapt<CreateProductInput>()));


        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update a exist product", Tags = new[] { "Products" })]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Produces(MediaTypeNames.Application.Json, "application/problem+json")]
        public async Task<IActionResult> Update(long id, UpdateProductRequest request)
        {
            await _productService.UpdateAsync(id, request.Adapt<UpdateProductInput>());

            return NoContent();
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete a product", Tags = new[] { "Products" })]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Produces(MediaTypeNames.Application.Json, "application/problem+json")]
        public async Task<IActionResult> Delete(long id)
        {
            await _productService.DeleteAsync(id);

            return NoContent();
        }
    }
}
