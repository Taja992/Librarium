using Librarium.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Librarium.Data;

public class LibrariumDbContext(DbContextOptions<LibrariumDbContext> options) : DbContext(options)
{
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Loan> Loans => Set<Loan>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LibrariumDbContext).Assembly);
    }
}
