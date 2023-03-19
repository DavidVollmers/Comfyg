using Comfyg.Contracts.Authentication;
using Microsoft.Extensions.Configuration;

namespace Comfyg.Authentication;

internal class SystemClient : IClient
{
    public SystemClient(IConfiguration configuration)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));
        
        ClientId = configuration["ComfygSystemClientId"];
        ClientSecret = configuration["ComfygSystemClientSecret"];
        FriendlyName = "Comfyg System Client";
    }

    public string ClientId { get; }

    public string ClientSecret { get; }

    public string FriendlyName { get; }

    // ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    public bool IsConfigured => ClientId != null && ClientSecret != null;
    // ReSharper enable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
}