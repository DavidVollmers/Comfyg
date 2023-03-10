using Comfyg.Api.Contracts.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Comfyg.Api.Controllers;

[Authorize]
[ApiController]
[Route("connections")]
public class ConnectionController : ControllerBase
{
    [HttpPost("establish")]
    public async Task<ActionResult<ConnectionResponse>> EstablishConnectionAsync()
    {
        return Ok(new ConnectionResponse());
    }
}