namespace Librarium.Api.Requests;

public record CreateLoanRequest(Guid BookId, Guid MemberId);
