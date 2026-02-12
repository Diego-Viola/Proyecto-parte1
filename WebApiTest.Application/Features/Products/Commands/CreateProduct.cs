using MediatR;
using WebApiTest.Application.DTOs.Inputs.Products;
using WebApiTest.Application.DTOs.Outputs.Categories;
using WebApiTest.Application.DTOs.Outputs.Products;
using WebApiTest.Application.Exceptions;
using WebApiTest.Application.Interfaces.IRepositories;
using WebApiTest.Domain.Exceptions;
using WebApiTest.Domain.Models;

namespace WebApiTest.Application.Features.Products.Commands;
public record CreateProduct(CreateProductInput Input) : IRequest<ProductDetailOutput> { }

public class CreateProductHandler : IRequestHandler<CreateProduct, ProductDetailOutput>
{
    private readonly IProductRepository productRepository;
    private readonly ICategoryRepository categoryRepository;

    public CreateProductHandler(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        this.productRepository = productRepository;
        this.categoryRepository = categoryRepository;
    }

    public async Task<ProductDetailOutput> Handle(CreateProduct request, CancellationToken cancellationToken)
    {
        if (request.Input.Price <= 0)
            throw new BusinessException("El precio del producto debe ser mayor a cero.", "API-CP-01");

        if (request.Input.Stock < 0)
            throw new BusinessException("El stock del producto no puede ser negativo.", "API-CP-02");

        var category = await categoryRepository.GetByIdAsync(request.Input.CategoryId)
            ?? throw new BadRequestException("La categoría del producto no fue encontrada", "API-CP-03");

        var product = await productRepository.AddAsync(new Product
        {
            Name = request.Input.Name,
            Description = request.Input.Description,
            Price = request.Input.Price,
            Stock = request.Input.Stock,
            CategoryId = request.Input.CategoryId
        });

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
            }
        };
    }
}
