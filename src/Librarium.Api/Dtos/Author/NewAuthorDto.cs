namespace Librarium.Api.Dtos.Author;

/// <summary>
/// Data required to create a new author.
/// </summary>
public record NewAuthorDto(string FirstName, string LastName, string? Biography);
