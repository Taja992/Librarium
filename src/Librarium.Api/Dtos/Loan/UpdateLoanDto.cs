namespace Librarium.Api.Dtos.Loan;

/// <summary>
/// Data required to update a loan (e.g. mark as returned).
/// </summary>
public record UpdateLoanDto(DateOnly? ReturnDate);
