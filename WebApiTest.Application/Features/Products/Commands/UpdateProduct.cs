using MediatR;
using WebApiTest.Application.DTOs.Inputs.Products;
using WebApiTest.Application.Exceptions;
using WebApiTest.Application.Interfaces.IRepositories;
using WebApiTest.Domain.Exceptions;

namespace WebApiTest.Application.Features.Products.Commands;
public record UpdateProduct(long productId, UpdateProductInput Input) : IRequest { }

public class UpdateProductHandler : IRequestHandler<UpdateProduct>
{
    private readonly IProductRepository productRepository;
    private readonly ICategoryRepository categoryRepository;

    public UpdateProductHandler(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        this.productRepository = productRepository;
        this.categoryRepository = categoryRepository;
    }

    public async Task Handle(UpdateProduct request, CancellationToken cancellationToken)
    {
        if (request.Input.Price <= 0)
            throw new BusinessException("El precio del producto debe ser mayor a cero.", "API-UP-01");

        if (request.Input.Stock < 0)
            throw new BusinessException("El stock del producto no puede ser negativo.", "API-UP-02");

        var product = await productRepository.GetByIdAsync(request.productId)
            ?? throw new NotFoundException("El producto no fue encontrado", "API-UP-03");

        if (product.CategoryId != request.Input.CategoryId)
        {
            var category = await categoryRepository.GetByIdAsync(request.Input.CategoryId)
                ?? throw new BadRequestException("La categoría del producto no fue encontrada", "API-UP-04");

            product.CategoryId = request.Input.CategoryId;
        }

        product.Name = request.Input.Name;
        product.Description = request.Input.Description;
        product.Price = request.Input.Price;
        product.Stock = request.Input.Stock;

        await productRepository.UpdateAsync(product);
    }
}
