using Comfyg.Store.Api.Responses;
using Comfyg.Store.Authentication.Abstractions;
using Comfyg.Store.Contracts.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Comfyg.Store.Api.Controllers;

[Authorize]
[ApiController]
[Route("connections")]
public class ConnectionController : ControllerBase
{
    [HttpPost("establish")]
    public ActionResult<IConnectionResponse> EstablishConnection()
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        return Ok(new ConnectionResponse(clientIdentity.Client));
    }
}
