namespace Comfyg.Store.Contracts;

public interface IComfygValueInitializer : IComfygValue
{
    /// <inheritdoc cref="IComfygValue"/>
    new string Key { get; init; }

    /// <inheritdoc cref="IComfygValue"/>
    new string Value { get; init; }

    /// <inheritdoc cref="IComfygValue"/>
    new string Version { get; init; }

    /// <inheritdoc cref="IComfygValue"/>
    new DateTimeOffset CreatedAt { get; init; }

    /// <inheritdoc cref="IComfygValue"/>
    new string? Hash { get; init; }
    
    /// <inheritdoc cref="IComfygValue"/>
    new string? ParentVersion { get; init; }
    
    /// <inheritdoc cref="IComfygValue"/>
    new bool IsEncrypted { get; init; }
}
