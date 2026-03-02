namespace Librarium.Api.Dtos.Member;

/// <summary>
/// Represents a library member.
/// </summary>
public record MemberDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    DateTime CreatedAt
);
