using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using WebApiTest.Application.DTOs.Generics;
using WebApiTest.Application.DTOs.Outputs.Categories;
using WebApiTest.Application.DTOs.Outputs.Products;
using WebApiTest.Application.Features.Categories.Commands;
using WebApiTest.Application.Features.Categories.Queries;
using WebApiTest.Controllers.Requests;

namespace WebApiTest.Controllers
{
    public class CategoriesController : BaseApiController
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
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
            var categories = await _mediator.Send(new GetCategories(count, page));

            return categories == null || !categories.Items.Any() ? NoContent() : Ok(categories);
        }


        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Return category detail", Tags = new[] { "Categories" })]
        [ProducesResponseType(typeof(CategoryOutput), StatusCodes.Status200OK)]
        [Produces(MediaTypeNames.Application.Json, "application/problem+json")]
        public async Task<IActionResult> GetById(long id)
            => Ok(await _mediator.Send(new GetCategoryDetail(id)));


        [HttpPost]
        [SwaggerOperation(Summary = "Create a new category", Tags = new[] { "Categories" })]
        [ProducesResponseType(typeof(ProductDetailOutput), StatusCodes.Status201Created)]
        [Produces(MediaTypeNames.Application.Json, "application/problem+json")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
            => StatusCode(201, await _mediator.Send(new CreateCategory(request.Name)));

    }
}
