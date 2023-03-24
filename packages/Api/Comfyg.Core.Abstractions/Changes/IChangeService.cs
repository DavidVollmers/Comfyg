using Comfyg.Contracts.Changes;

namespace Comfyg.Core.Abstractions.Changes;

public interface IChangeService
{
    Task LogChangeAsync<T>(string targetId, ChangeType changeType, string changedBy,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<IChangeLog> GetChangesSinceAsync<T>(DateTimeOffset since,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<IChangeLog> GetChangesForOwnerAsync<T>(string owner, DateTimeOffset since,
        CancellationToken cancellationToken = default);
}
