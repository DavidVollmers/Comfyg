namespace Comfyg.Store.Contracts;

/// <summary>
/// Comfyg value abstraction which allows to initialize its properties.
/// </summary>
public interface IComfygValueInitializer : IComfygValue
{
    /// <inheritdoc cref="IComfygValue.Key"/>
    new string Key { get; init; }

    /// <inheritdoc cref="IComfygValue.Value"/>
    new string Value { get; init; }

    /// <inheritdoc cref="IComfygValue.Version"/>
    new string Version { get; init; }

    /// <inheritdoc cref="IComfygValue.CreatedAt"/>
    new DateTimeOffset CreatedAt { get; init; }

    /// <inheritdoc cref="IComfygValue.Hash"/>
    new string? Hash { get; init; }
    
    /// <inheritdoc cref="IComfygValue.ParentVersion"/>
    new string? ParentVersion { get; init; }
    
    /// <inheritdoc cref="IComfygValue.IsEncrypted"/>
    new bool IsEncrypted { get; init; }
}
