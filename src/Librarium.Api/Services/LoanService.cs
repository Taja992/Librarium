using Librarium.Api.Dtos.Loan;
using Librarium.Data.Entities;
using Librarium.Data.Interfaces;

namespace Librarium.Api.Services;

public class LoanService : Interfaces.ILoanService
{
    private readonly ILoanRepository _loanRepository;

    public LoanService(ILoanRepository loanRepository)
    {
        _loanRepository = loanRepository;
    }

    public async Task<Loan> CreateLoanAsync(NewLoanDto dto)
    {
        var loan = new Loan { BookId = dto.BookId, MemberId = dto.MemberId };
        return await _loanRepository.AddAsync(loan);
    }

    public async Task<IEnumerable<Loan>> GetLoansByMemberAsync(Guid memberId)
    {
        return await _loanRepository.GetByMemberIdAsync(memberId);
    }
}
