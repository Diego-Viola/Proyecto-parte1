using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using Products.Api.Application.DTOs.Outputs.Categories;
using Products.Api.Application.DTOs.Generics;
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

        /// <summary>
        /// Obtiene una lista paginada de categorías.
        /// </summary>
        /// <param name="count">Cantidad de categorías por página (default: 20)</param>
        /// <param name="page">Número de página (default: 1)</param>
        [HttpGet]
        [SwaggerOperation(Summary = "Obtiene lista paginada de categorías", Tags = new[] { "Categories" })]
        [ProducesResponseType(typeof(PaginationResult<CategoryOutput>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Produces(MediaTypeNames.Application.Json, "application/problem+json")]
        public async Task<IActionResult> GetAll(
            [FromQuery, Range(1, 100)] int count = 20,
            [FromQuery, Range(1, int.MaxValue)] int page = 1
        )
        {
            var categories = await _categoryService.GetAllAsync(count, page);

            return !categories.Items.Any() ? NoContent() : Ok(categories);
        }

        /// <summary>
        /// Obtiene el detalle de una categoría por su ID.
        /// </summary>
        /// <param name="id">ID de la categoría</param>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Obtiene detalle de categoría", Tags = new[] { "Categories" })]
        [ProducesResponseType(typeof(CategoryOutput), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json, "application/problem+json")]
        public async Task<IActionResult> GetById(long id)
            => Ok(await _categoryService.GetByIdAsync(id));

        /// <summary>
        /// Crea una nueva categoría.
        /// </summary>
        [HttpPost]
        [SwaggerOperation(Summary = "Crea una nueva categoría", Tags = new[] { "Categories" })]
        [ProducesResponseType(typeof(CategoryOutput), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [Produces(MediaTypeNames.Application.Json, "application/problem+json")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
            => StatusCode(201, await _categoryService.CreateAsync(request.Name));

    }
}
