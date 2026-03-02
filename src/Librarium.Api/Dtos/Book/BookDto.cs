using Librarium.Api.Dtos.Author;

namespace Librarium.Api.Dtos.Book;

/// <summary>
/// Represents a book.
/// </summary>
public record BookDto(
    Guid Id,
    string Title,
    string Isbn,
    short PublishedYear,
    DateTime CreatedAt,
    IReadOnlyList<AuthorDto> Authors
);
