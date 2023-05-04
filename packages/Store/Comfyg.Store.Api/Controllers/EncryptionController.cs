using Comfyg.Store.Authentication.Abstractions;
using Comfyg.Store.Contracts.Requests;
using Comfyg.Store.Core.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Comfyg.Store.Api.Controllers;

[Authorize]
[ApiController]
[Route("encryption")]
public class EncryptionController : ControllerBase
{
    private readonly IClientService _clientService;

    public EncryptionController(IClientService clientService)
    {
        _clientService = clientService;
    }

    [HttpGet("key")]
    public async Task<IActionResult> GetEncryptionKeyAsync(CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity {Client.IsAsymmetric: true} clientIdentity) return Forbid();

        var encryptionKey = await _clientService.GetEncryptionKeyAsync(clientIdentity.Client, cancellationToken);

        if (encryptionKey == null) return NotFound();
        
        return File(encryptionKey, "application/octet-stream");
    }
}
