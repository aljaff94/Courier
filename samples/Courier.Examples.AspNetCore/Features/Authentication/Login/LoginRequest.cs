using Microsoft.AspNetCore.Mvc;

namespace Courier.Examples.AspNetCore.Features.Authentication.Login;

public class LoginRequest
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}