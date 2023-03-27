namespace Comfyg.Store.Core.Abstractions.Changes;

internal class ChangeLog : IChangeLog
{
    public Type TargetType { get; init; } = null!;

    public string TargetId { get; init; } = null!;

    public DateTimeOffset ChangedAt { get; init; } = DateTimeOffset.UtcNow;

    public ChangeType ChangeType { get; init; } = ChangeType.Unknown;

    public string ChangedBy { get; init; } = null!;
}
