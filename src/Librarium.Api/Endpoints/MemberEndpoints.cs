using Librarium.Api.Interfaces;

namespace Librarium.Api.Endpoints;

public static class MemberEndpoints
{
    public static RouteGroupBuilder MapMemberEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api/members").WithTags("Members");

        api.MapGet("/", GetAllMembers)
            .WithName("Get all members")
            .WithDescription("Returns a list of all members.");

        return api;
    }

    private static async Task<IResult> GetAllMembers(IMemberService memberService)
    {
        var result = await memberService.GetAllMembersAsync();
        return Results.Ok(result);
    }
}
