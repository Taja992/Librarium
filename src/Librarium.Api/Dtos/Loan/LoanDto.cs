namespace Librarium.Api.Dtos.Loan;

/// <summary>
/// Represents a loan.
/// </summary>
public record LoanDto(
    Guid LoanId,
    string BookTitle,
    DateOnly LoanDate,
    DateOnly? ReturnDate,
    string Status // additive — existing frontend clients ignore this until they are ready
);
