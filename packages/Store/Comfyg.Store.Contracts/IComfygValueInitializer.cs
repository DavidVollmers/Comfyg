namespace Comfyg.Store.Contracts;

public interface IComfygValueInitializer : IComfygValue
{
    new string Key { get; init; }

    new string Value { get; init; }

    new string Version { get; init; }

    new DateTimeOffset CreatedAt { get; init; }

    new string? Hash { get; init; }
    
    new string? ParentVersion { get; init; }
    
    new bool IsEncrypted { get; init; }
}
