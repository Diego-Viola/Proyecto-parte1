using MediatR;
using WebApiTest.Application.DTOs.Outputs.Categories;
using WebApiTest.Application.Exceptions;
using WebApiTest.Application.Interfaces.IRepositories;

namespace WebApiTest.Application.Features.Categories.Queries;
public record GetCategoryDetail(long categoryId) : IRequest<CategoryOutput> { }

public class GetCategoryDetailHandler : IRequestHandler<GetCategoryDetail, CategoryOutput>
{
    private readonly ICategoryRepository categoryRepository;

    public GetCategoryDetailHandler(ICategoryRepository categoryRepository)
    {
        this.categoryRepository = categoryRepository;
    }

    public async Task<CategoryOutput> Handle(GetCategoryDetail request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.categoryId);

        if (category is null) throw new NotFoundException("La categoria solicitada no existe", "API-GCD-01");

        return new CategoryOutput
        {
            Id = category.Id,
            Name = category.Name
        };
    }
}