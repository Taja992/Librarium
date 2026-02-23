using Librarium.Data.Entities;

namespace Librarium.Data.Interfaces;

public interface IMemberRepository
{
    Task<IEnumerable<Member>> GetAllAsync();
}
