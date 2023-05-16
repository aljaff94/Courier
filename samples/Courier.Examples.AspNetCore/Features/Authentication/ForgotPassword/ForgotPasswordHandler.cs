using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Courier.Examples.AspNetCore.Features.Authentication.ForgotPassword
{
    [Endpoint(HttpMethods.Post, "/forgot-password/{id}")]
    [Authorize]
    public class ForgotPasswordHandler : ICommandHandler<Guid, ForgotPasswordRequest, ForgotPasswordResponse>
    {
        public Task<ForgotPasswordResponse> HandleAsync([FromRoute]Guid id, [FromQuery]ForgotPasswordRequest request, CancellationToken ctx)
        {
            throw new NotImplementedException();
        }
    }
}
