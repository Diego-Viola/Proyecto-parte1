using Products.Api.Application.DTOs.Generics;
using Products.Api.Application.DTOs.Inputs.Products;
using Products.Api.Application.DTOs.Outputs.Products;

namespace Products.Api.Application.Interfaces.IServices;

public interface IProductService
{
    Task<PaginationResult<ProductOutput>> GetAllAsync(GetProductsInput input);
    Task<ProductDetailOutput> GetByIdAsync(long id);
    Task<ProductDetailOutput> CreateAsync(CreateProductInput input);
    Task UpdateAsync(long productId, UpdateProductInput input);
    Task DeleteAsync(long productId);
}
