namespace Librarium.Data.Entities;

public class Book
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Isbn { get; set; } = string.Empty;
    public short PublishedYear { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public ICollection<Loan> Loans { get; set; } = [];

    public ICollection<Author> Authors { get; set; } = [];
}
