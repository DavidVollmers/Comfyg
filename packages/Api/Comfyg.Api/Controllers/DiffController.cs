using Comfyg.Authentication.Abstractions;
using Comfyg.Contracts.Configuration;
using Comfyg.Contracts.Secrets;
using Comfyg.Contracts.Settings;
using Comfyg.Core.Abstractions.Changes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Comfyg.Api.Controllers;

[Authorize]
[ApiController]
[Route("diff")]
public class DiffController : ControllerBase
{
    private readonly IChangeService _changeService;

    public DiffController(IChangeService changeService)
    {
        _changeService = changeService;
    }

    [HttpGet("configuration")]
    public IActionResult GetConfigurationDiffAsync([FromQuery] DateTime since,
        CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var changes =
            _changeService.GetChangesForOwnerAsync<IConfigurationValue>(clientIdentity.Client.ClientId, since,
                cancellationToken);

        return Ok(changes);
    }

    [HttpGet("settings")]
    public IActionResult GetSettingsDiffAsync([FromQuery] DateTime since, CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var changes =
            _changeService.GetChangesForOwnerAsync<ISettingValue>(clientIdentity.Client.ClientId, since,
                cancellationToken);

        return Ok(changes);
    }

    [HttpGet("secrets")]
    public IActionResult GetSecretsDiffAsync([FromQuery] DateTime since, CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var changes =
            _changeService.GetChangesForOwnerAsync<ISecretValue>(clientIdentity.Client.ClientId, since,
                cancellationToken);

        return Ok(changes);
    }
}
