namespace Librarium.Api.Dtos.Loan;

/// <summary>
/// Represents a loan.
/// </summary>
public record LoanDto(Guid Id, Guid BookId, Guid MemberId, DateOnly LoanDate, DateOnly? ReturnDate);
