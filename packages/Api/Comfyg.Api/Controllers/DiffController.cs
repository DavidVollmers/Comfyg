using Comfyg.Authentication.Abstractions;
using Comfyg.Contracts.Configuration;
using Comfyg.Contracts.Responses;
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
    public async Task<ActionResult<GetDiffResponse>> GetConfigurationDiffAsync([FromQuery] DateTime since)
    {
        return await CalculateDiffAsync<IConfigurationValue>(since);
    }

    [HttpGet("settings")]
    public async Task<ActionResult<GetDiffResponse>> GetSettingsDiffAsync([FromQuery] DateTime since)
    {
        return await CalculateDiffAsync<ISettingValue>(since);
    }

    [HttpGet("secrets")]
    public async Task<ActionResult<GetDiffResponse>> GetSecretsDiffAsync([FromQuery] DateTime since)
    {
        return await CalculateDiffAsync<ISecretValue>(since);
    }

    private async Task<ActionResult<GetDiffResponse>> CalculateDiffAsync<T>(DateTime since)
    {
        if (User.Identity is not IClientIdentity clientIdentity) return Forbid();

        var changes =
            await _changeService.GetChangesForOwnerAsync<T>(clientIdentity.Client.ClientId, since)
                .ConfigureAwait(false);

        return Ok(new GetDiffResponse(changes));
    }
}