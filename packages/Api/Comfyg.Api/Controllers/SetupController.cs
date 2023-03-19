using Comfyg.Authentication.Abstractions;
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
        var systemClient = _configuration["ComfygSystemClientId"];
        if (systemClient == null || User.Identity is not IClientIdentity identity) return BadRequest();

        if (identity.Client.ClientId != systemClient) return Forbid();

        var existing = await _clientService.GetClientAsync(request.Client.ClientId).ConfigureAwait(false);
        if (existing != null) return BadRequest();

        var client = await _clientService.CreateClientAsync(request.Client).ConfigureAwait(false);

        var clientSecret = await _clientService.ReceiveClientSecretAsync(client).ConfigureAwait(false);

        return Ok(new SetupClientResponse(client, clientSecret));
    }
}