using Librarium.Data.Entities;
using Librarium.Data.Interfaces;

namespace Librarium.Api.Services;

public class BookService : Interfaces.IBookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<IEnumerable<Book>> GetAllBooksAsync()
    {
        return await _bookRepository.GetAllAsync();
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
