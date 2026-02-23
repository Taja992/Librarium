using Librarium.Api.Interfaces;

namespace Librarium.Api.Endpoints;

public static class BookEndpoints
{
    public static RouteGroupBuilder MapBookEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api/books").WithTags("Books");

        api.MapGet("/", GetAllBooks)
            .WithName("Get all books")
            .WithDescription("Returns a list of all books.");

        return api;
    }

    private static async Task<IResult> GetAllBooks(IBookService bookService)
    {
        var result = await bookService.GetAllBooksAsync();
        return Results.Ok(result);
    }
}
