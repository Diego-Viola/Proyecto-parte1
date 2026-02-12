using WebApiTest.Application.DTOs.Generics;
using WebApiTest.Application.Interfaces.IRepositories;
using WebApiTest.Domain.Models;
using WebApiTest.Persistence.Entities;
using WebApiTest.Persistence.Interfaces;

namespace WebApiTest.Persistence.Repositories;
public class CategoryRepository : ICategoryRepository
{
    private readonly CustomContext _context;
    private readonly IAdapter<CategoryEntity, Category> _adapter;
    private static readonly object _lock = new();

    public CategoryRepository(CustomContext customContext, IAdapter<CategoryEntity, Category> adapter)
    {
        _context = customContext;
        _adapter = adapter;
    }

    public Task<Category> AddAsync(Category category)
    {
        lock (_lock)
        {
            category.Id = _context.Categories.Any() ? _context.Categories.Max(p => p.Id) + 1 : 1;

            var categoryEntity = new CategoryEntity
            {
                Id = category.Id,
                Name = category.Name
            };

            _context.Categories.Add(categoryEntity);
            _context.SaveChanges();

            return Task.FromResult(category);
        }
    }

    public Task<PaginationResult<Category>> GetAllAsync(int count, int page)
    {
        var query = _context.Categories.AsQueryable();
        var total = query.Count();

        var categories = query
            .Skip((page - 1) * count)
            .Take(count)
            .Select(x => _adapter.ToDomainModel(x))
            .ToList();

        return Task.FromResult(new PaginationResult<Category>
        {
            Items = categories,
            Total = total
        });
    }

    public Task<Category?> GetByIdAsync(long id)
    {
        var category = _context.Categories.FirstOrDefault(p => p.Id == id);

        return Task.FromResult(category is null ? null : _adapter.ToDomainModel(category));
    }

    public Task<Category?> GetByNameAsync(string name)
    {
        var category = _context.Categories.FirstOrDefault(p =>
            string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(category is null ? null : _adapter.ToDomainModel(category));
    }
}
