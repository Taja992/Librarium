using Librarium.Api.Dtos.Author;

namespace Librarium.Api.Dtos.Book;

/// <summary>
/// Represents a book with the corrected string ISBN. Migrate to this from v1.
/// </summary>
public record BookV2Dto(
    Guid Id,
    string Title,
    string? Isbn,
    short PublishedYear,
    DateTime CreatedAt,
    IReadOnlyList<AuthorDto> Authors
);
