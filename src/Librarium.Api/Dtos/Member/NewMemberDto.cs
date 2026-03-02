namespace Librarium.Api.Dtos.Member;

/// <summary>
/// Data required to register a new member.
/// </summary>
public record NewMemberDto(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber // required for new registrations would be enforced on application layer via validation
);
