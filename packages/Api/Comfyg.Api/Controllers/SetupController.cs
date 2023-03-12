using Comfyg.Authentication.Abstractions;
using Comfyg.Contracts.Authentication;
using Comfyg.Contracts.Requests;
using Comfyg.Contracts.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Comfyg.Api.Controllers;

[Authorize]
[ApiController]
[Route("setup")]
public class SetupController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IClientService _clientService;

    public SetupController(IConfiguration configuration, IClientService clientService)
    {
        _configuration = configuration;
        _clientService = clientService;
    }

    [HttpPost("client")]
    public async Task<ActionResult<SetupClientResponse>> SetupClientAsync([FromBody] SetupClientRequest request)
    {
        var systemClient = _configuration["ComfygSystemClient"];
        if (systemClient == null || User.Identity is not IClientIdentity identity) return BadRequest();

        if (identity.Client.ClientId != systemClient) return Forbid();

        var existing = await _clientService.GetClientAsync(request.Client.ClientId).ConfigureAwait(false);
        if (existing != null) return BadRequest();

        await _clientService.CreateClientAsync(request.Client).ConfigureAwait(false);

        return Ok();
    }
}