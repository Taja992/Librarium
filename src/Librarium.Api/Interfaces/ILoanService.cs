using Librarium.Api.Requests;
using Librarium.Data.Entities;

namespace Librarium.Api.Interfaces;

public interface ILoanService
{
    Task<Loan> CreateLoanAsync(CreateLoanRequest request);
    Task<IEnumerable<Loan>> GetLoansByMemberAsync(Guid memberId);
}
