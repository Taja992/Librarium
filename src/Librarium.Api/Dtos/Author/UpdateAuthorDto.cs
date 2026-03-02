namespace Librarium.Api.Dtos.Author;

/// <summary>
/// Data required to update an existing author.
/// </summary>
public record UpdateAuthorDto(string FirstName, string LastName, string? Biography);
