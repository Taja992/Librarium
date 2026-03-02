namespace Librarium.Api.Dtos.Member;

/// <summary>
/// Data required to register a new member.
/// </summary>
public record NewMemberDto(string FirstName, string LastName, string Email);
