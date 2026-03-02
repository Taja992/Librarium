using Librarium.Api.Dtos.Loan;
using Librarium.Api.Interfaces;

namespace Librarium.Api.Endpoints;

public static class LoanEndpoints
{
    public static RouteGroupBuilder MapLoanEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api/loans").WithTags("Loans");

        api.MapPost("/", CreateLoan)
            .WithName("Create loan")
            .WithDescription("Creates a new loan record for a member borrowing a book.");

        api.MapGet("/{memberId:guid}", GetLoansByMember)
            .WithName("Get loans by member")
            .WithDescription("Returns all loans for a given member.");

        return api;
    }

    private static async Task<IResult> CreateLoan(NewLoanDto dto, ILoanService loanService)
    {
        var result = await loanService.CreateLoanAsync(dto);
        return Results.Created($"/api/loans/{result.Id}", result);
    }

    private static async Task<IResult> GetLoansByMember(Guid memberId, ILoanService loanService)
    {
        var result = await loanService.GetLoansByMemberAsync(memberId);
        return Results.Ok(result);
    }
}
