using Comfyg.Store.Contracts.Secrets;

namespace Comfyg.Tests.Common.Contracts;

public class TestSecretValue : ISecretValue
{
    public string Key { get; set; } = null!;

    public string Value { get; set; } = null!;

    public string Version { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public string Hash { get; set; } = null!;
}
