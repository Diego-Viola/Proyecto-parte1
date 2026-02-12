using WebApiTest.Application.DTOs.Generics;
using WebApiTest.Application.DTOs.Outputs.Categories;

namespace WebApiTest.Application.Interfaces.IServices;

public interface ICategoryService
{
    Task<PaginationResult<CategoryOutput>> GetAllAsync(int count, int page);
    Task<CategoryOutput> GetByIdAsync(long id);
    Task<CategoryOutput> CreateAsync(string name);
}
