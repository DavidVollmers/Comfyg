using Comfyg.Contracts;

namespace Comfyg.Core.Abstractions;

public interface IValueService<T> where T : IComfygValue
{
    Task AddValueAsync(string owner, string key, string value);

    Task<IEnumerable<T>> GetValuesAsync(string owner);

    Task<T?> GetValueAsync(string key, string version = null!);
}