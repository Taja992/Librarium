using Librarium.Api.Dtos.Book;

namespace Librarium.Api.Interfaces;

public interface IBookService
{
    Task<IEnumerable<BookDto>> GetAllBooksAsync();
    Task<IEnumerable<BookV2Dto>> GetAllBooksV2Async();
}
