namespace Products.Api.Application.DTOs.Outputs.Generics;

public class PaginationResult<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int Total { get; set; }
}