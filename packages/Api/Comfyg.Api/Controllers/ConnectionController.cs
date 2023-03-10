using Comfyg.Api.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Comfyg.Api.Controllers;

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