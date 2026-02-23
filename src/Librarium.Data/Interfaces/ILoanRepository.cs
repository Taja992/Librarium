using Librarium.Data.Entities;

namespace Librarium.Data.Interfaces;

public interface ILoanRepository
{
    Task<Loan> AddAsync(Loan entity);
    Task<IEnumerable<Loan>> GetByMemberIdAsync(Guid memberId);
}
