namespace Librarium.Api.Dtos.Book;

/// <summary>
/// Data required to create a new book.
/// </summary>
public record NewBookDto(
    string Title,
    string Isbn,
    short PublishedYear,
    IReadOnlyList<Guid> AuthorIds
);
