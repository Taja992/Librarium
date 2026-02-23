using Librarium.Api.Configuration;
using Scalar.AspNetCore;

namespace Librarium.Api.Extensions;

public static class ApiDocumentationExtensions
{
    public static IEndpointRouteBuilder UseApiDocumentation(
        this WebApplication app,
        LibrariumOptions options
    )
    {
        app.MapOpenApi();
        app.MapScalarApiReference(scalar =>
        {
            scalar.Title = $"{options.ApplicationName} API";
            scalar.Theme = ScalarTheme.DeepSpace;
        });

        var logger = app.Services.GetRequiredService<ILogger<WebApplication>>();
        app.Lifetime.ApplicationStarted.Register(() =>
        {
            var address = app.Urls.FirstOrDefault() ?? "http://localhost:5240";
            logger.LogInformation("Scalar API Reference: {Address}/scalar/v1", address);
        });

        return app;
    }
}
