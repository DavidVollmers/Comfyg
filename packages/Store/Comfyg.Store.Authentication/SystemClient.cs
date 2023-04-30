using Comfyg.Store.Contracts;
using Microsoft.Extensions.Configuration;

namespace Comfyg.Store.Authentication;

internal class SystemClient : IClient
{
    public SystemClient(IConfiguration configuration)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        ClientId = configuration["SystemClientId"];
        ClientSecret = configuration["SystemClientSecret"];
        FriendlyName = "Comfyg System Client";
    }

    public string ClientId { get; }

    public string ClientSecret { get; }

    public string FriendlyName { get; }

    public bool IsAsymmetric => false;

    // ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    public bool IsConfigured => ClientId != null && ClientSecret != null;
    // ReSharper enable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
}
