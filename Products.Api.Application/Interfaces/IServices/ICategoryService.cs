using Products.Api.Application.DTOs.Outputs.Categories;
using Products.Api.Application.DTOs.Generics;

namespace Products.Api.Application.Interfaces.IServices;

public interface ICategoryService
{
    Task<PaginationResult<CategoryOutput>> GetAllAsync(int count, int page);
    Task<CategoryOutput> GetByIdAsync(long id);
    Task<CategoryOutput> CreateAsync(string name);
}
