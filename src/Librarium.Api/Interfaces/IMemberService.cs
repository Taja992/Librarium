using Librarium.Data.Entities;

namespace Librarium.Api.Interfaces;

public interface IMemberService
{
    Task<IEnumerable<Member>> GetAllMembersAsync();
}
