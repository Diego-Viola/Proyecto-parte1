using MediatR;
using WebApiTest.Application.DTOs.Outputs.Categories;
using WebApiTest.Application.Interfaces.IRepositories;
using WebApiTest.Domain.Exceptions;
using WebApiTest.Domain.Models;

namespace WebApiTest.Application.Features.Categories.Commands;
public record CreateCategory(string name) : IRequest<CategoryOutput> { }

public class CreateCategoryHandler : IRequestHandler<CreateCategory, CategoryOutput>
{
    private readonly ICategoryRepository _categoryRepository;

    public CreateCategoryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryOutput> Handle(CreateCategory request, CancellationToken cancellationToken)
    {
        if (await _categoryRepository.GetByNameAsync(request.name) is not null)
            throw new BusinessException("Ya existe una categoria con el mismo nombre", "ATI-CC-01");

        var category = await _categoryRepository.AddAsync(new Category { Name = request.name });

        return new CategoryOutput { Id = category.Id, Name = category.Name };
    }
}
