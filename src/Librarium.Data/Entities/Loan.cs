namespace Librarium.Data.Entities;

public class Loan
{
    public Guid Id { get; set; }
    public Guid BookId { get; set; }
    public Guid MemberId { get; set; }
    public DateOnly LoanDate { get; set; }
    public DateOnly? ReturnDate { get; set; }

    public Book Book { get; set; } = null!;
    public Member Member { get; set; } = null!;
}
