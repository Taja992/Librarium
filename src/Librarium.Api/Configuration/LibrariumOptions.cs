namespace Librarium.Api.Configuration;

public class LibrariumOptions
{
    public string ApplicationName { get; set; } = "Librarium";
    public string[] AllowedOrigins { get; set; } = [];
}
