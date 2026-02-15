using Products.Api.Application.DTOs.Outputs.Generics;
using Products.Api.Domain.Models;

namespace Products.Api.Application.Interfaces.IRepositories;
public interface IProductRepository
{
    Task<PaginationResult<Product>> GetAllAsync(int page, int count, string? name, long? categoryId);
    Task<Product?> GetByIdAsync(long id);
    Task<Product> AddAsync(Product product);
    Task<bool> UpdateAsync(Product product);
    Task<bool> DeleteAsync(long id);
}
