using Librarium.Data.Entities;

namespace Librarium.Api.Interfaces;

public interface IBookService
{
    Task<IEnumerable<Book>> GetAllBooksAsync();
}
