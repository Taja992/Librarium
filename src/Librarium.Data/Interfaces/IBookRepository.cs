using Librarium.Data.Entities;

namespace Librarium.Data.Interfaces;

public interface IBookRepository
{
    Task<IEnumerable<Book>> GetAllAsync();
}
