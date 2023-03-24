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
    public async Task<ActionResult<SetupClientResponse>> SetupClientAsync([FromBody] SetupClientRequest request,
        CancellationToken cancellationToken = default)
    {
        var systemClient = _configuration["SystemClientId"];
        if (systemClient == null || User.Identity is not IClientIdentity identity) return BadRequest();

        if (identity.Client.ClientId != systemClient) return Forbid();

        var existing = await _clientService.GetClientAsync(request.Client.ClientId, cancellationToken)
            ;
        if (existing != null) return BadRequest();

        var client = await _clientService.CreateClientAsync(request.Client, cancellationToken);

        var clientSecret =
            await _clientService.ReceiveClientSecretAsync(client, cancellationToken);

        return Ok(new SetupClientResponse(client, clientSecret));
    }
}
