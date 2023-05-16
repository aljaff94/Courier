using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Courier.Examples.AspNetCore.Features.Authentication.Login;

[Endpoint(HttpMethods.Post, "/login")]
[AllowAnonymous]
public class LoginHandler : ICommandHandler<LoginRequest, LoginResponse>
{
    public LoginHandler()
    {
        
    }
    
    public Task<LoginResponse> HandleAsync([FromQuery]LoginRequest request, CancellationToken ctx)
    {
        throw new NotImplementedException();
    }
}