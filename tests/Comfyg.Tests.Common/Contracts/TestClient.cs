using Comfyg.Store.Contracts;

namespace Comfyg.Tests.Common.Contracts;

public class TestClient : IClient
{
    public string ClientId { get; set; } = null!;

    public string ClientSecret { get; set; } = null!;

    public string FriendlyName { get; set; } = null!;
    
    public bool IsAsymmetric { get; set; }
}
