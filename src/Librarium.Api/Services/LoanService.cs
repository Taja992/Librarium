using Librarium.Api.Dtos.Loan;
using Librarium.Api.Interfaces;
using Librarium.Data.Entitie.Enum;
using Librarium.Data.Entities;
using Librarium.Data.Interfaces;

namespace Librarium.Api.Services;

public class LoanService : ILoanService
{
    private readonly ILoanRepository _loanRepository;

    public LoanService(ILoanRepository loanRepository)
    {
        _loanRepository = loanRepository;
    }

    public async Task<Loan> CreateLoanAsync(NewLoanDto dto)
    {
        // Below is what would be set up these methods existed

        // var book = await _bookRepository.GetByIdAsync(dto.BookId);

        // if (book is null || book.IsDeleted)
        //     throw new InvalidOperationException("This book has been retired and cannot be loaned.");

        var loan = new Loan
        {
            BookId = dto.BookId,
            MemberId = dto.MemberId,
            Status = LoanStatus.Active,
        };

        return await _loanRepository.AddAsync(loan);
    }

    public async Task<IEnumerable<LoanDto>> GetLoansByMemberAsync(Guid memberId)
    {
        var loans = await _loanRepository.GetByMemberIdAsync(memberId);
        return loans.Select(l => new LoanDto(
            l.Id,
            l.Book.Title,
            l.LoanDate,
            l.ReturnDate,
            l.Status.ToString() ?? "Active"
        ));
    }
}
