using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using Products.Api.Application.DTOs.Outputs.Categories;
using Products.Api.Application.DTOs.Generics;
using Products.Api.Application.DTOs.Outputs.Products;
using Products.Api.Application.Interfaces.IServices;
using Products.Api.Controllers.Requests;

namespace Products.Api.Controllers
{
    public class CategoriesController : BaseApiController
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Return all categories", Tags = new[] { "Categories" })]
        [ProducesResponseType(typeof(PaginationResult<CategoryOutput>), StatusCodes.Status200OK)]
        [Produces(MediaTypeNames.Application.Json, "application/problem+json")]
        public async Task<IActionResult> GetAll(
            [FromQuery, Required, Range(1, int.MaxValue)] int count,
            [FromQuery, Required, Range(1, int.MaxValue)] int page
        )
        {
            var categories = await _categoryService.GetAllAsync(count, page);

            return !categories.Items.Any() ? NoContent() : Ok(categories);
        }


        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Return category detail", Tags = new[] { "Categories" })]
        [ProducesResponseType(typeof(CategoryOutput), StatusCodes.Status200OK)]
        [Produces(MediaTypeNames.Application.Json, "application/problem+json")]
        public async Task<IActionResult> GetById(long id)
            => Ok(await _categoryService.GetByIdAsync(id));


        [HttpPost]
        [SwaggerOperation(Summary = "Create a new category", Tags = new[] { "Categories" })]
        [ProducesResponseType(typeof(ProductDetailOutput), StatusCodes.Status201Created)]
        [Produces(MediaTypeNames.Application.Json, "application/problem+json")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
            => StatusCode(201, await _categoryService.CreateAsync(request.Name));

    }
}
