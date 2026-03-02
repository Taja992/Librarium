using Librarium.Api.Dtos.Member;

namespace Librarium.Api.Interfaces;

public interface IMemberService
{
    Task<IEnumerable<MemberDto>> GetAllMembersAsync();
}
