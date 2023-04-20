﻿using Comfyg.Store.Api.Responses;
using Comfyg.Store.Authentication.Abstractions;
using Comfyg.Store.Contracts.Requests;
using Comfyg.Store.Contracts.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Comfyg.Store.Api.Controllers;

[Authorize]
[ApiController]
[Route("setup")]
public class SetupController : ControllerBase
{
    private readonly IClientService _clientService;

    public SetupController(IClientService clientService)
    {
        _clientService = clientService;
    }

    [HttpPost("client")]
    public async Task<ActionResult<ISetupClientResponse>> SetupClientAsync(
        [FromForm(Name = nameof(ISetupClientRequest))]
        ISetupClientRequest.Form request, CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity identity) return BadRequest();

        if (!identity.IsSystemClient) return Forbid();

        var existing = await _clientService.GetClientAsync(request.ClientId, cancellationToken);
        if (existing != null) return BadRequest();

        if (request.ClientSecretPublicKey == null)
        {
            var client = await _clientService.CreateClientAsync(request, cancellationToken);

            var clientSecret =
                await _clientService.ReceiveClientSecretAsync(client, cancellationToken);

            return Ok(new SetupClientResponse(client, clientSecret));
        }

        //TODO store public key as client secret and use for JWT signature verification...
        throw new NotImplementedException();
    }
}
