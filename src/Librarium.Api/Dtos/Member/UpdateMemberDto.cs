namespace Librarium.Api.Dtos.Member;

/// <summary>
/// Data required to update an existing member.
/// </summary>
public record UpdateMemberDto(string FirstName, string LastName, string Email);
