using Librarium.Api.Dtos.Author;
using Librarium.Api.Dtos.Book;
using Librarium.Data.Interfaces;

namespace Librarium.Api.Services;

public class BookService : Interfaces.IBookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    // v1 — reads IsbnLegacy so existing consumers receive the truncated integer value
    // they had before the column type change rather than a sudden null.
    public async Task<IEnumerable<BookDto>> GetAllBooksAsync()
    {
        var books = await _bookRepository.GetAllAsync();
        return books.Select(b => new BookDto(
            b.Id,
            b.Title,
            b.IsbnLegacy, // uses legacy for old endpoint
            b.PublishedYear,
            b.CreatedAt,
            b.Authors.Select(a => new AuthorDto(a.Id, a.FirstName, a.LastName, a.Biography))
                .ToList()
        ));
    }

    // v2 — reads the corrected string Isbn column. Will be null until data is manually corrected.
    public async Task<IEnumerable<BookV2Dto>> GetAllBooksV2Async()
    {
        var books = await _bookRepository.GetAllAsync();
        return books.Select(b => new BookV2Dto(
            b.Id,
            b.Title,
            b.Isbn,
            b.PublishedYear,
            b.CreatedAt,
            b.Authors.Select(a => new AuthorDto(a.Id, a.FirstName, a.LastName, a.Biography))
                .ToList()
        ));
    }

    // If this endpoint was here, something like this or just an interceptor interface

    // public async Task RetireBookAsync(Guid id)
    // {
    //     var book = await _bookRepository.GetByIdAsync(id);

    //     if (book is null)
    //         throw new KeyNotFoundException("Book not found.");

    //     if (book.IsDeleted)
    //         return; // already retired, no-op

    //     book.IsDeleted = true;
    //     await _bookRepository.UpdateAsync(book);
    // }
}
