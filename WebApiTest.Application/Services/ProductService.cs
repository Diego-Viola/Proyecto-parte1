﻿using WebApiTest.Application.DTOs.Generics;
using WebApiTest.Application.DTOs.Inputs.Products;
using WebApiTest.Application.DTOs.Outputs.Categories;
using WebApiTest.Application.DTOs.Outputs.Products;
using WebApiTest.Application.Exceptions;
using WebApiTest.Application.Interfaces.IRepositories;
using WebApiTest.Application.Interfaces.IServices;
using WebApiTest.Domain.Exceptions;
using WebApiTest.Domain.Models;

namespace WebApiTest.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<PaginationResult<ProductOutput>> GetAllAsync(GetProductsInput input)
    {
        var result = await _productRepository
            .GetAllAsync(input.Page, input.Count, input.Name, input.CategoryId);

        return new PaginationResult<ProductOutput>
        {
            Items = result.Items.Select(x => new ProductOutput
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.Price,
                CategoryId = x.CategoryId
            }),
            Total = result.Total
        };
    }

    public async Task<ProductDetailOutput> GetByIdAsync(long id)
    {
        var product = await _productRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("El producto no fue encontrado con el id especificado", "API-GPD-01");

        var category = await _categoryRepository.GetByIdAsync(product.CategoryId)
            ?? throw new DataIntegrationException("La categoría del producto registrado no fue encontrada");

        return new ProductDetailOutput
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            Category = new CategoryOutput
            {
                Id = category.Id,
                Name = category.Name
            },
        };
    }

    public async Task<ProductDetailOutput> CreateAsync(CreateProductInput input)
    {
        if (input.Price <= 0)
            throw new BusinessException("El precio del producto debe ser mayor a cero.", "API-CP-01");

        if (input.Stock < 0)
            throw new BusinessException("El stock del producto no puede ser negativo.", "API-CP-02");

        var category = await _categoryRepository.GetByIdAsync(input.CategoryId)
            ?? throw new BadRequestException("La categoría del producto no fue encontrada", "API-CP-03");

        var product = await _productRepository.AddAsync(new Product
        {
            Name = input.Name,
            Description = input.Description,
            Price = input.Price,
            Stock = input.Stock,
            CategoryId = input.CategoryId
        });

        return new ProductDetailOutput
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            Category = new CategoryOutput
            {
                Id = category.Id,
                Name = category.Name
            }
        };
    }

    public async Task UpdateAsync(long productId, UpdateProductInput input)
    {
        if (input.Price <= 0)
            throw new BusinessException("El precio del producto debe ser mayor a cero.", "API-UP-01");

        if (input.Stock < 0)
            throw new BusinessException("El stock del producto no puede ser negativo.", "API-UP-02");

        var product = await _productRepository.GetByIdAsync(productId)
            ?? throw new NotFoundException("El producto no fue encontrado", "API-UP-03");

        if (product.CategoryId != input.CategoryId)
        {
            _ = await _categoryRepository.GetByIdAsync(input.CategoryId)
                ?? throw new BadRequestException("La categoría del producto no fue encontrada", "API-UP-04");

            product.CategoryId = input.CategoryId;
        }

        product.Name = input.Name;
        product.Description = input.Description;
        product.Price = input.Price;
        product.Stock = input.Stock;

        await _productRepository.UpdateAsync(product);
    }

    public async Task DeleteAsync(long productId)
    {
        _ = await _productRepository.GetByIdAsync(productId)
            ?? throw new NotFoundException("El producto no fue encontrado", "API-DP-01");

        await _productRepository.DeleteAsync(productId);
    }
}
