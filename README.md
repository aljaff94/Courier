# Courier

Source Generator For .NET Minimal API.
```
Experimental project, not ready for production.
```


## Installation
```bash
dotnet tool install Courier.SourceGeneration
```


## Usage
in Program.cs
```csharp
using Courier;
// ....

var builder = WebApplication.CreateBuilder(args);
// ....
builder.Services.AddCourier();
// ....
var app = builder.Build();
// ....
app.UseCourier();
// ....
app.Run();

```
## Type of handlers
* Command Handler
```csharp
namespace Example;

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class LoginResponse
{
    public string Token { get; set; }
}

[Endpoint(HttpMethods.Post, "/login")]
[AllowAnonymous]
public class LoginHandler : ICommandHandler<LoginRequest, LoginResponse>
{
    public Task<LoginResponse> HandleAsync([FromBody]LoginRequest request, CancellationToken ctx)
    {
    }
}
```
* Command Handler with Id
```csharp
namespace Example;

public class EditBillRequest
{
    public decimal Amount { get; set; }
}

public class EditBillResponse
{
    public string Message { get; set; }
}

[Authorize]
[Endpoint(HttpMethods.Put, "/bills/{id:Guid}")]
public class EditBillHandler : ICommandHandler<Guid, EditBillRequest, EditBillResponse>
{
    public Task<LoginResponse> HandleAsync(Guid id, [FromBody]LoginRequest request, CancellationToken ctx)
    {
    }
}
```
* Query Handler with optional id
```csharp
namespace Example;

public class GetBillsResponse
{
    public string Message { get; set; }
}

[Authorize]
[Endpoint(HttpMethods.Get, "/bills/{id?:Guid}")]
public class GetBillsResponseHandler : IQueryHandler<Guid?, GetBillsResponse>
{
    public Task<GetBillsResponse> HandleAsync([FromRoute]Guid? id, CancellationToken ctx)
    {
    }
}
```

## Generated Code
```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Example.Features.Authentication.Login;
using Example.Features.Users.GetUser;
using Example.Features.Authentication.ForgotPassword;

namespace Courier
{
    public static class Extensions
    {
        public static IServiceCollection AddCourier(this IServiceCollection services)
        {
            services.AddTransient<LoginHandler>();
            services.AddTransient<EditBillHandler>();
            services.AddTransient<GetBillsHandler>();

            return services;
        }

        public static WebApplication UseCourier(this WebApplication app)
        {
            app.MapPost("/login", [AllowAnonymous]([FromBody] LoginRequest request, CancellationToken ctx, [FromServices] LoginHandler handler) => handler.HandleAsync(request, ctx));
            app.MapPut("/bills/{id:Guid}", [Authorize](Guid id, CancellationToken ctx, [FromServices] EditBillHandler handler) => handler.HandleAsync(id, ctx));
            app.MapGet("/bills/{id?}", [Authorize]([FromRoute] Guid? id, CancellationToken ctx, [FromServices] GetBillsHandler handler) => handler.HandleAsync(id, ctx));

            return app;
        }
    }
}
```