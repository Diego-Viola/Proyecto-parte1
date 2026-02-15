using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using Products.Api.Application.DTOs.Inputs.Products;
using Products.Api.Application.DTOs.Outputs.Generics;
using Products.Api.Application.DTOs.Outputs.ProductDetail;
using Products.Api.Application.DTOs.Outputs.Products;
using Products.Api.Application.Helpers;
using Products.Api.Application.Interfaces.IServices;

namespace Products.Api.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Obtiene una lista paginada de productos con filtros opcionales.
        /// </summary>
        /// <param name="count">Cantidad de productos por página (default: 20)</param>
        /// <param name="page">Número de página (default: 1)</param>
        /// <param name="name">Filtro por nombre (opcional)</param>
        /// <param name="categoryId">Filtro por categoría (opcional)</param>
        [HttpGet]
        [SwaggerOperation(Summary = "Obtiene lista paginada de productos", Tags = new[] { "Products" })]
        [ProducesResponseType(typeof(PaginationResult<ProductOutput>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Produces(MediaTypeNames.Application.Json, "application/problem+json")]
        public async Task<IActionResult> GetAll(
            [FromQuery, Range(1, 100)] int count = 20,
            [FromQuery, Range(1, int.MaxValue)] int page = 1,
            [FromQuery] string? name = null,
            [FromQuery] long? categoryId = null)
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

        /// <summary>
        /// Obtiene el detalle básico de un producto por su ID.
        /// </summary>
        /// <param name="id">ID del producto</param>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Obtiene detalle básico del producto", Tags = new[] { "Products" })]
        [ProducesResponseType(typeof(ProductDetailOutput), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json, "application/problem+json")]
        public async Task<IActionResult> GetById(long id)
            => Ok(await _productService.GetByIdAsync(id));

        /// <summary>
        /// Obtiene el detalle completo y enriquecido de un producto para una página de detalle de marketplace.
        /// Incluye: imágenes, vendedor, envío, variantes, atributos, ratings, productos relacionados.
        /// </summary>
        /// <param name="id">ID del producto</param>
        [HttpGet("{id}/detail")]
        [SwaggerOperation(
            Summary = "Obtiene detalle completo del producto",
            Description = "Retorna toda la información necesaria para renderizar una página de detalle de producto completa, incluyendo imágenes, vendedor, envío, variantes, atributos técnicos, ratings y productos relacionados.",
            Tags = new[] { "Products" })]
        [ProducesResponseType(typeof(ProductDetailEnrichedOutput), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json, "application/problem+json")]
        public async Task<IActionResult> GetDetailById(long id)
        {
            // Obtener producto básico
            var basicProduct = await _productService.GetByIdAsync(id);

            // Enriquecer con datos adicionales de marketplace
            // En producción, esto vendría de múltiples servicios/repositorios
            var enrichedProduct = ProductEnricherHelper.EnrichProduct(basicProduct);

            return Ok(enrichedProduct);
        }

        /// <summary>
        /// Obtiene productos relacionados a un producto específico.
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <param name="limit">Cantidad máxima de productos relacionados (default: 6)</param>
        [HttpGet("{id}/related")]
        [SwaggerOperation(
            Summary = "Obtiene productos relacionados",
            Description = "Retorna una lista de productos relacionados basados en la categoría y comportamiento de otros usuarios.",
            Tags = new[] { "Products" })]
        [ProducesResponseType(typeof(List<ProductSummaryOutput>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json, "application/problem+json")]
        public async Task<IActionResult> GetRelatedProducts(long id, [FromQuery, Range(1, 20)] int limit = 6)
        {
            // Validar que el producto existe (lanza NotFoundException si no existe)
            _ = await _productService.GetByIdAsync(id);

            // Generar productos relacionados simulados
            var random = new Random((int)id);
            var relatedProducts = Enumerable.Range(1, limit)
                .Select(i =>
                {
                    var relatedId = ((id + i) % 100) + 1;
                    // Evitar retornar el mismo producto
                    if (relatedId == id) relatedId = ((id + i + 1) % 100) + 1;

                    return new ProductSummaryOutput
                    {
                        Id = relatedId,
                        Name = $"Producto Relacionado {relatedId}",
                        Price = random.Next(1000, 50000),
                        ThumbnailUrl = $"https://cdn.marketplace.com/products/{relatedId}/thumb-1.jpg",
                        Rating = Math.Round(3.5m + (decimal)random.NextDouble() * 1.5m, 1),
                        FreeShipping = random.Next(100) < 40
                    };
                })
                .ToList();

            return Ok(relatedProducts);
        }

        /// <summary>
        /// Crea un nuevo producto.
        /// </summary>
        [HttpPost]
        [SwaggerOperation(Summary = "Crea un nuevo producto", Tags = new[] { "Products" })]
        [ProducesResponseType(typeof(ProductDetailOutput), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [Produces(MediaTypeNames.Application.Json, "application/problem+json")]
        public async Task<IActionResult> Create([FromBody] CreateProductInput request)
            => StatusCode(201, await _productService.CreateAsync(request));

        /// <summary>
        /// Actualiza un producto existente.
        /// </summary>
        /// <param name="id">ID del producto a actualizar</param>
        /// <param name="request">Datos actualizados del producto</param>
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Actualiza un producto existente", Tags = new[] { "Products" })]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json, "application/problem+json")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateProductInput request)
        {
            await _productService.UpdateAsync(id, request);
            return NoContent();
        }

        /// <summary>
        /// Elimina un producto.
        /// </summary>
        /// <param name="id">ID del producto a eliminar</param>
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Elimina un producto", Tags = new[] { "Products" })]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json, "application/problem+json")]
        public async Task<IActionResult> Delete(long id)
        {
            await _productService.DeleteAsync(id);

            return NoContent();
        }
    }
}
