namespace Librarium.Data.Entities;

public class Member
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public ICollection<Loan> Loans { get; set; } = [];
}
