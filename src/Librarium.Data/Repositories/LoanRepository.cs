using Librarium.Data.Entities;
using Librarium.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Librarium.Data.Repositories;

public class LoanRepository(LibrariumDbContext context) : ILoanRepository
{
    public async Task<IEnumerable<Loan>> GetAllAsync() => await context.Loans.ToListAsync();

    public async Task<Loan> AddAsync(Loan entity)
    {
        context.Loans.Add(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<IEnumerable<Loan>> GetByMemberIdAsync(Guid memberId) =>
        await context.Loans.Where(l => l.MemberId == memberId).ToListAsync();
}
