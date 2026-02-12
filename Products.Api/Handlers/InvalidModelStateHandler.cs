using Microsoft.AspNetCore.Mvc;
using Products.Api.Exceptions;

namespace Products.Api.Handlers;

public static class InvalidModelStateHandler
{
    public static IActionResult Handle(ActionContext context)
    {
        var errors = context.ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );

        throw new InputException(errors);
    }
}