using Products.Api.Application.DTOs.Outputs.Categories;
using Products.Api.Application.DTOs.Generics;
using Products.Api.Application.Exceptions;
using Products.Api.Application.Interfaces.IRepositories;
using Products.Api.Application.Interfaces.IServices;
using Products.Api.Domain.Exceptions;
using Products.Api.Domain.Models;

namespace Products.Api.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<PaginationResult<CategoryOutput>> GetAllAsync(int count, int page)
    {
        var categories = await _categoryRepository.GetAllAsync(count, page);

        return new PaginationResult<CategoryOutput>
        {
            Items = categories.Items.Select(x => new CategoryOutput
            {
                Id = x.Id,
                Name = x.Name
            }),
            Total = categories.Total
        };
    }

    public async Task<CategoryOutput> GetByIdAsync(long id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category is null) 
            throw new NotFoundException("La categoria solicitada no existe", "API-GCD-01");

        return new CategoryOutput
        {
            Id = category.Id,
            Name = category.Name
        };
    }

    public async Task<CategoryOutput> CreateAsync(string name)
    {
        if (await _categoryRepository.GetByNameAsync(name) is not null)
            throw new BusinessException("Ya existe una categoria con el mismo nombre", "ATI-CC-01");

        var category = await _categoryRepository.AddAsync(new Category { Name = name });

        return new CategoryOutput { Id = category.Id, Name = category.Name };
    }
}
