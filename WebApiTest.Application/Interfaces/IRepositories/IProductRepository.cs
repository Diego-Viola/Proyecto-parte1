
using WebApiTest.Application.DTOs.Generics;
using WebApiTest.Domain.Models;

namespace WebApiTest.Application.Interfaces.IRepositories;
public interface IProductRepository
{
    Task<PaginationResult<Product>> GetAllAsync(int page, int count, string name, long? category_id);
    Task<Product?> GetByIdAsync(long id);
    Task<Product> AddAsync(Product product);
    Task<bool> UpdateAsync(Product product);
    Task<bool> DeleteAsync(long id);
}
