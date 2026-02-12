using WebApiTest.Application.DTOs.Generics;
using WebApiTest.Application.DTOs.Inputs.Products;
using WebApiTest.Application.DTOs.Outputs.Products;

namespace WebApiTest.Application.Interfaces.IServices;

public interface IProductService
{
    Task<PaginationResult<ProductOutput>> GetAllAsync(GetProductsInput input);
    Task<ProductDetailOutput> GetByIdAsync(long id);
    Task<ProductDetailOutput> CreateAsync(CreateProductInput input);
    Task UpdateAsync(long productId, UpdateProductInput input);
    Task DeleteAsync(long productId);
}
