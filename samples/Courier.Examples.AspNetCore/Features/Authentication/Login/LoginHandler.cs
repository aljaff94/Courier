using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Courier.Examples.AspNetCore.Features.Authentication.Login;

[Endpoint(
    HttpMethods.Post, "/login",
    Name = "Login via Username and Password",
    Description = """
        Authenticate user using username and password.
        Returns a JWT token that can be used to authenticate future requests.
    """,
    Summary = "Some Summary Here",
    Tags = new[] { "Authentication" },
    UseOpenApi = true
    )]
[AllowAnonymous]
[ProducesResponseType(typeof(LoginResponse), 200)]
[ProducesResponseType(typeof(ProblemDetails), 400)]
public class LoginHandler : ICommandHandler<LoginRequest, LoginResponse>
{
    public LoginHandler()
    {
        
    }
    
    public Task<LoginResponse> HandleAsync([FromBody]LoginRequest request, CancellationToken ctx)
    {
        throw new NotImplementedException();
    }
}