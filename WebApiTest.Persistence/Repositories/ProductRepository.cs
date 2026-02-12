using WebApiTest.Application.DTOs.Generics;
using WebApiTest.Application.Interfaces.IRepositories;
using WebApiTest.Domain.Models;
using WebApiTest.Persistence.Entities;
using WebApiTest.Persistence.Interfaces;

namespace WebApiTest.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly CustomContext _context;
    private readonly IAdapter<ProductEntity, Product> _adapter;
    private static readonly object _lock = new();

    public ProductRepository(CustomContext context, IAdapter<ProductEntity, Product> adapter)
    {
        _context = context;
        _adapter = adapter;
    }

    public Task<PaginationResult<Product>> GetAllAsync(int page, int count, string name, long? categoryId)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(p => p.Name != null && p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        var total = query.Count();

        var products = query
            .Skip((page - 1) * count)
            .Take(count)
            .ToList()
            .Select(x => _adapter.ToDomainModel(x))
            .ToList();

        return Task.FromResult(new PaginationResult<Product>
        {
            Items = products,
            Total = total
        });
    }

    public Task<Product?> GetByIdAsync(long id)
    {
        var product = _context.Products.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(product is null ? null : _adapter.ToDomainModel(product));
    }

    public Task<Product> AddAsync(Product product)
    {
        lock (_lock)
        {
            product.Id = _context.Products.Any() ? _context.Products.Max(p => p.Id) + 1 : 1;

            var productEntity = new ProductEntity
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryId = product.CategoryId
            };

            _context.Products.Add(productEntity);
            _context.SaveChanges();

            return Task.FromResult(product);
        }
    }

    public Task<bool> UpdateAsync(Product product)
    {
        lock (_lock)
        {
            var entity = _context.Products.FirstOrDefault(p => p.Id == product.Id);
            if (entity is null) return Task.FromResult(false);

            entity.Name = product.Name;
            entity.Description = product.Description;
            entity.Price = product.Price;
            entity.Stock = product.Stock;
            entity.CategoryId = product.CategoryId;

            _context.SaveChanges();
            return Task.FromResult(true);
        }
    }

    public Task<bool> DeleteAsync(long id)
    {
        lock (_lock)
        {
            var entity = _context.Products.FirstOrDefault(p => p.Id == id);

            if (entity is null) return Task.FromResult(false);

            _context.Products.Remove(entity);
            _context.SaveChanges();

            return Task.FromResult(true);
        }
    }
}
