using Librarium.Api.Dtos.Member;
using Librarium.Data.Interfaces;

namespace Librarium.Api.Services;

public class MemberService : Interfaces.IMemberService
{
    private readonly IMemberRepository _memberRepository;

    public MemberService(IMemberRepository memberRepository)
    {
        _memberRepository = memberRepository;
    }

    public async Task<IEnumerable<MemberDto>> GetAllMembersAsync()
    {
        var members = await _memberRepository.GetAllAsync();
        return members.Select(m => new MemberDto(
            m.Id,
            m.FirstName,
            m.LastName,
            m.Email,
            m.PhoneNumber,
            m.CreatedAt
        ));
    }
}
