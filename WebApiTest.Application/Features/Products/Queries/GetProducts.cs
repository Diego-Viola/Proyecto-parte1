using MediatR;
using WebApiTest.Application.DTOs.Generics;
using WebApiTest.Application.DTOs.Inputs.Products;
using WebApiTest.Application.DTOs.Outputs.Products;
using WebApiTest.Application.Interfaces.IRepositories;

namespace WebApiTest.Application.Features.Products.Queries;
public record GetProducts(GetProductsInput input) : IRequest<PaginationResult<ProductOutput>> { }

public class GetProductsHandler : IRequestHandler<GetProducts, PaginationResult<ProductOutput>>
{
    private readonly IProductRepository productRepository;

    public GetProductsHandler(IProductRepository productRepository)
    {
        this.productRepository = productRepository;
    }

    public async Task<PaginationResult<ProductOutput>> Handle(GetProducts request, CancellationToken cancellationToken)
    {
        var filters = request.input;

        var result = await productRepository
            .GetAllAsync(filters.Page, filters.Count, filters.Name, filters.CategoryId);

        return new PaginationResult<ProductOutput>
        {
            Items = result.Items.Select(x => new ProductOutput
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.Price,
                CategoryId = x.CategoryId
            }),
            Total = result.Total
        };
    }
}
