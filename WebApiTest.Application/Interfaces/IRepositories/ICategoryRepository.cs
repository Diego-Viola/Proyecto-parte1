using WebApiTest.Application.DTOs.Generics;
using WebApiTest.Domain.Models;

namespace WebApiTest.Application.Interfaces.IRepositories;
public interface ICategoryRepository
{
    Task<PaginationResult<Category>> GetAllAsync(int count, int page);
    Task<Category?> GetByIdAsync(long id);
    Task<Category?> GetByNameAsync(string name);
    Task<Category> AddAsync(Category category);
}
