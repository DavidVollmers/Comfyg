using Comfyg.Contracts.Configuration;

namespace Comfyg.Core.Abstractions.Configuration;

public interface IConfigurationService
{
    Task AddConfigurationValueAsync(string owner, string key, string value);

    Task<IEnumerable<IConfigurationValue>> GetConfigurationValuesAsync(string owner);

    Task<IConfigurationValue?> GetConfigurationValueAsync(string key, string version = null!);
}