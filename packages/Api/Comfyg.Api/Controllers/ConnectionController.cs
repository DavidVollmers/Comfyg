using Comfyg.Authentication.Abstractions;
using Comfyg.Contracts.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Comfyg.Api.Controllers;

[Authorize]
[ApiController]
[Route("connections")]
public class ConnectionController : ControllerBase
{
    [HttpPost("establish")]
    public ActionResult<ConnectionResponse> EstablishConnection()
    {
        if (User.Identity is not IClientIdentity clientIdentity) return BadRequest();

        return Ok(new ConnectionResponse(clientIdentity.Client));
    }
}