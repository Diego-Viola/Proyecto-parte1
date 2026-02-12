using MediatR;
using WebApiTest.Application.DTOs.Outputs.Categories;
using WebApiTest.Application.DTOs.Outputs.Products;
using WebApiTest.Application.Exceptions;
using WebApiTest.Application.Interfaces.IRepositories;

namespace WebApiTest.Application.Features.Products.Queries;

public record GetProductDetail(long productId) : IRequest<ProductDetailOutput> { }

public class GetProductDetailHandler : IRequestHandler<GetProductDetail, ProductDetailOutput>
{
    private readonly IProductRepository productRepository;
    private readonly ICategoryRepository categoryRepository;

    public GetProductDetailHandler(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        this.productRepository = productRepository;
        this.categoryRepository = categoryRepository;
    }

    public async Task<ProductDetailOutput> Handle(GetProductDetail request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.productId)
            ?? throw new NotFoundException("El producto no fue encontrado con el id especificado", "API-GPD-01");

        var category = await categoryRepository.GetByIdAsync(product.CategoryId)
            ?? throw new DataIntegrationException("La categoría del producto registrado no fue encontrada");

        return new ProductDetailOutput
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            Category = new CategoryOutput
            {
                Id = category.Id,
                Name = category.Name
            },
        };
    }
}
