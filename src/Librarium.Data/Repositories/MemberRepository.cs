using Librarium.Data.Entities;
using Librarium.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Librarium.Data.Repositories;

public class MemberRepository(LibrariumDbContext context) : IMemberRepository
{
    public async Task<IEnumerable<Member>> GetAllAsync() => await context.Members.ToListAsync();

    public async Task<Member> AddAsync(Member entity)
    {
        context.Members.Add(entity);
        await context.SaveChangesAsync();
        return entity;
    }
}
