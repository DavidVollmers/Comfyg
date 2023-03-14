using Comfyg.Authentication.Abstractions;
using Comfyg.Contracts.Configuration;
using Comfyg.Contracts.Requests;
using Comfyg.Core.Abstractions.Configuration;
using Comfyg.Core.Abstractions.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Comfyg.Api.Controllers;

[Authorize]
[ApiController]
[Route("configuration")]
public class ConfigurationController : ControllerBase
{
    private readonly IConfigurationService _configurationService;
    private readonly IPermissionService _permissionService;

    public ConfigurationController(IConfigurationService configurationService, IPermissionService permissionService)
    {
        _configurationService = configurationService;
        _permissionService = permissionService;
    }

    [HttpPost]
    public async Task<ActionResult> AddConfigurationAsync([FromBody] AddConfigurationRequest request)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        foreach (var configurationValue in request.ConfigurationValues)
        {
            var isPermitted = await _permissionService
                .IsPermittedAsync<IConfigurationValue>(clientIdentity.Client.ClientId, configurationValue.Key)
                .ConfigureAwait(false);
            if (!isPermitted) return Forbid();
        }

        foreach (var configurationValue in request.ConfigurationValues)
        {
            await _configurationService
                .AddConfigurationValueAsync(clientIdentity.Client.ClientId, configurationValue.Key,
                    configurationValue.Value)
                .ConfigureAwait(false);
        }

        return Ok();
    }
}