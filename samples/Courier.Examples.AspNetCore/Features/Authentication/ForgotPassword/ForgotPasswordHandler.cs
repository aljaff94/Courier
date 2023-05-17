using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Courier.Examples.AspNetCore.Features.Authentication.ForgotPassword
{
    [Endpoint(HttpMethods.Post, "/forgot-password/{id}",
        Name = "Reset Password",
        Description = """
            Reset a user's password.
            This endpoint is used to reset a user's password when they have forgotten it.

        """,
        Tags = new[] { "Authentication" },
        Summary = "Reset a user's password"
        )]
    public class ForgotPasswordHandler : ICommandHandler<Guid, ForgotPasswordRequest, ForgotPasswordResponse>
    {
        public Task<ForgotPasswordResponse> HandleAsync([FromRoute] Guid id, [FromBody] ForgotPasswordRequest request, CancellationToken ctx)
        {
            throw new NotImplementedException();
        }
    }
}
