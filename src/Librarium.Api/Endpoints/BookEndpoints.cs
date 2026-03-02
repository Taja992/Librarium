using Librarium.Api.Interfaces;

namespace Librarium.Api.Endpoints;

public static class BookEndpoints
{
    public static RouteGroupBuilder MapBookEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api/books").WithTags("Books");
        var apiV2 = app.MapGroup("/api/v2/books").WithTags("Books V2");

        // v1 — deprecated. ISBN is returned from IsbnLegacy (truncated integer cast to string).
        // Migrate to /api/v2/books before v1 is retired.
        api.MapGet("/", GetAllBooks)
            .WithName("Get all books (deprecated)")
            .WithDescription(
                "DEPRECATED: ISBN is now returned as a string from IsbnLegacy. Migrate to /api/v2/books."
            )
            .WithSummary("[DEPRECATED] Returns a list of all books.");

        // v2 — corrected string ISBN. Will be null per book until manually corrected.
        apiV2
            .MapGet("/", GetAllBooksV2)
            .WithName("Get all books")
            .WithDescription(
                "Returns a list of all books with the corrected string ISBN. "
                    + "Isbn will be null for books whose ISBN has not yet been manually corrected."
            );

        return api;
    }

    private static async Task<IResult> GetAllBooks(IBookService bookService)
    {
        var result = await bookService.GetAllBooksAsync();
        return Results.Ok(result);
    }

    private static async Task<IResult> GetAllBooksV2(IBookService bookService)
    {
        var result = await bookService.GetAllBooksV2Async();
        return Results.Ok(result);
    }
}
