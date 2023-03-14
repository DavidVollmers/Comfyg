using Comfyg.Contracts.Settings;

namespace Comfyg.Core.Abstractions.Settings;

public interface ISettingService
{
    Task AddSettingValueAsync(string owner, string key, string value);

    Task<IEnumerable<ISettingValue>> GetSettingValuesAsync(string owner);

    Task<ISettingValue?> GetSettingValueAsync(string key, string version = null!);
}