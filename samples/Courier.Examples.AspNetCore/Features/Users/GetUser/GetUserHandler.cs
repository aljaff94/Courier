using Microsoft.AspNetCore.Authorization;

namespace Courier.Examples.AspNetCore.Features.Users.GetUser;

[Authorize(Roles = "admin")]
[Endpoint(HttpMethods.Get, "/users/{id}",
        Name = "Get user by id",
        Description = """
            Get a user by their id.
            This endpoint is used to get a user by their id.
        """,
        Tags = new[] { "Users" },
        Summary = "Get user by id"
    )]
public class GetUserHandler : IQueryHandler<Guid, GetUserResponse>
{
    public Task<GetUserResponse> HandleAsync(Guid id, CancellationToken ctx)
    {
        throw new NotImplementedException();
    }
}