using Comfyg.Contracts.Changes;

namespace Comfyg.Core.Abstractions.Changes;

public interface IChangeService
{
    Task LogChangeAsync<T>(string targetId, ChangeType changeType, string changedBy);

    Task<IEnumerable<IChangeLog>> GetChangesSinceAsync<T>(DateTime since);
}