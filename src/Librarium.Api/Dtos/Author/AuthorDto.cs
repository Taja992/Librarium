namespace Librarium.Api.Dtos.Author;

/// <summary>
/// Represents an author.
/// </summary>
public record AuthorDto(Guid Id, string FirstName, string LastName, string? Biography);
