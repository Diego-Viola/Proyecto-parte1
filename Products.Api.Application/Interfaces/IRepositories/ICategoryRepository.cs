using Products.Api.Application.DTOs.Generics;
using Products.Api.Domain.Models;

namespace Products.Api.Application.Interfaces.IRepositories;
public interface ICategoryRepository
{
    Task<PaginationResult<Category>> GetAllAsync(int count, int page);
    Task<Category?> GetByIdAsync(long id);
    Task<Category?> GetByNameAsync(string name);
    Task<Category> AddAsync(Category category);
}
