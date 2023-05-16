using Microsoft.AspNetCore.Authorization;

namespace Courier.Examples.AspNetCore.Features.Users.GetUser;

[Authorize(Roles = "admin")]
[Endpoint(HttpMethods.Get, "/users/{id}")]
public class GetUserHandler : IQueryHandler<Guid, GetUserResponse>
{
    public Task<GetUserResponse> HandleAsync(Guid id, CancellationToken ctx)
    {
        throw new NotImplementedException();
    }
}