using MediatR;
using WebApiTest.Application.Exceptions;
using WebApiTest.Application.Interfaces.IRepositories;

namespace WebApiTest.Application.Features.Products.Commands;
public record DeleteProduct(long productId) : IRequest { }

public class DeleteProductHandler : IRequestHandler<DeleteProduct>
{
    private readonly IProductRepository productRepository;

    public DeleteProductHandler(IProductRepository productRepository)
    {
        this.productRepository = productRepository;
    }

    public async Task Handle(DeleteProduct request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.productId)
            ?? throw new NotFoundException("El producto no fue encontrado", "API-DP-01");

        await productRepository.DeleteAsync(request.productId);
    }
}
