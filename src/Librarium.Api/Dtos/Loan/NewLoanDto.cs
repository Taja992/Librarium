namespace Librarium.Api.Dtos.Loan;

/// <summary>
/// Data required to create a new loan.
/// </summary>
public record NewLoanDto(Guid BookId, Guid MemberId);
