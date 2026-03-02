namespace Librarium.Api.Dtos.Book;

/// <summary>
/// Data required to update an existing book.
/// </summary>
public record UpdateBookDto(string Title, string Isbn, short PublishedYear);
