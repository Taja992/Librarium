namespace Librarium.Data.Entities;

public class Author
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Biography { get; set; }
    public ICollection<Book> Books { get; set; } = [];
}
