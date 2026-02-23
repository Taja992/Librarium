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
}
