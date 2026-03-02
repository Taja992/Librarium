using Librarium.Api.Dtos.Loan;
using Librarium.Data.Entities;

namespace Librarium.Api.Interfaces;

public interface ILoanService
{
    Task<Loan> CreateLoanAsync(NewLoanDto dto);
    Task<IEnumerable<LoanDto>> GetLoansByMemberAsync(Guid memberId);
}
