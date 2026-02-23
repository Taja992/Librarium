using Librarium.Api.Configuration;
using Librarium.Api.Endpoints;
using Librarium.Api.Extensions;
using Librarium.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// IOptions configuration
builder.Services.Configure<LibrariumOptions>(builder.Configuration.GetSection("LibrariumOptions"));

var librariumOptions = builder
    .Configuration.GetSection("LibrariumOptions")
    .Get<LibrariumOptions>()!;

// OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer(
        (document, context, cancellationToken) =>
        {
            document.Info.Title = $"{librariumOptions.ApplicationName} API";
            document.Info.Version = "v0.0.1";
            return Task.CompletedTask;
        }
    );
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "Development",
        policy =>
            policy.WithOrigins(librariumOptions.AllowedOrigins).AllowAnyMethod().AllowAnyHeader()
    );
    options.AddPolicy(
        "Production",
        policy =>
            policy.WithOrigins(librariumOptions.AllowedOrigins).AllowAnyMethod().AllowAnyHeader()
    );
});

// DbContext
builder.Services.AddDbContext<LibrariumDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Application services
builder.Services.AddApplication();

var app = builder.Build();

// This auto updates development database if a migration has been done
if (app.Environment.IsDevelopment())
{
    await using var scope = app.Services.CreateAsyncScope();
    var db = scope.ServiceProvider.GetRequiredService<LibrariumDbContext>();
    await db.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var corsPolicy = app.Environment.IsDevelopment() ? "Development" : "Production";
app.UseCors(corsPolicy);

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

// Endpoints
app.MapBookEndpoints();
app.MapMemberEndpoints();
app.MapLoanEndpoints();

app.Run();
