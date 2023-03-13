using Comfyg.Contracts.Configuration;

namespace Comfyg.Core.Abstractions.Configuration;

public interface IConfigurationService
{
    Task AddConfigurationAsync(string owner, string key, string value);

    Task<IEnumerable<IConfigurationValue>> GetConfigurationAsync(string owner);
}