using Librarium.Data.Entities;
using Librarium.Data.Interfaces;

namespace Librarium.Api.Services;

public class MemberService : Interfaces.IMemberService
{
    private readonly IMemberRepository _memberRepository;

    public MemberService(IMemberRepository memberRepository)
    {
        _memberRepository = memberRepository;
    }

    public async Task<IEnumerable<Member>> GetAllMembersAsync()
    {
        return await _memberRepository.GetAllAsync();
    }
}
