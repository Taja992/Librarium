using Librarium.Data.Entities;
using Librarium.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Librarium.Data.Repositories;

public class BookRepository(LibrariumDbContext context) : IBookRepository
{
    public async Task<IEnumerable<Book>> GetAllAsync() => await context.Books.ToListAsync();

    public async Task<Book> AddAsync(Book entity)
    {
        context.Books.Add(entity);
        await context.SaveChangesAsync();
        return entity;
    }
}
