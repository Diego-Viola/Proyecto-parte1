using MediatR;
using WebApiTest.Application.DTOs.Generics;
using WebApiTest.Application.DTOs.Outputs.Categories;
using WebApiTest.Application.Interfaces.IRepositories;

namespace WebApiTest.Application.Features.Categories.Queries;
public record GetCategories(int count, int page) : IRequest<PaginationResult<CategoryOutput>> { }

public class GetCategoriesHandler : IRequestHandler<GetCategories, PaginationResult<CategoryOutput>>
{
    private readonly ICategoryRepository categoryRepository;

    public GetCategoriesHandler(ICategoryRepository categoryRepository)
    {
        this.categoryRepository = categoryRepository;
    }

    public async Task<PaginationResult<CategoryOutput>> Handle(GetCategories request, CancellationToken cancellationToken)
    {
        var categories = await categoryRepository.GetAllAsync(request.count, request.page);

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
}