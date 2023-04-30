using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Comfyg.Store.Api.Responses;
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
    public async Task<ActionResult<ISetupClientResponse>> SetupClientAsync([FromForm] ISetupClientRequest.Form request,
        CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity identity) return BadRequest();

        if (!identity.IsSystemClient) return Forbid();

        var existing = await _clientService.GetClientAsync(request.ClientId, cancellationToken);
        if (existing != null) return BadRequest();

        if (!request.IsAsymmetric)
        {
            var symmetricClient = await _clientService.CreateSymmetricClientAsync(request, cancellationToken);

            var clientSecret =
                await _clientService.ReceiveClientSecretAsync(symmetricClient, cancellationToken);

            return Ok(new SetupClientResponse(symmetricClient, Convert.ToBase64String(clientSecret)));
        }

        using var stream = new MemoryStream();
        await request.ClientSecretPublicKey!.CopyToAsync(stream, cancellationToken);

        stream.Position = 0;

        using var rsa = RSA.Create();
        rsa.ImportRSAPublicKey(stream.ToArray(), out _);

        var asymmetricClient =
            await _clientService.CreateAsymmetricClientAsync(request, rsa, cancellationToken);

        return Ok(new SetupClientResponse(asymmetricClient));
    }
}
