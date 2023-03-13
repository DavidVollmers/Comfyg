using Comfyg.Authentication.Abstractions;
using Comfyg.Contracts.Requests;
using Comfyg.Core.Abstractions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Comfyg.Api.Controllers;

[Authorize]
[ApiController]
[Route("configuration")]
public class ConfigurationController : ControllerBase
{
    private readonly IConfigurationService _configurationService;

    public ConfigurationController(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
    }

    [HttpPost]
    public async Task<ActionResult> AddConfigurationAsync([FromBody] AddConfigurationRequest request)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        foreach (var configurationValue in request.ConfigurationValues)
        {
            await _configurationService
                .AddConfigurationAsync(clientIdentity.Client.ClientId, configurationValue.Key, configurationValue.Value)
                .ConfigureAwait(false);
        }

        return Ok();
    }
}