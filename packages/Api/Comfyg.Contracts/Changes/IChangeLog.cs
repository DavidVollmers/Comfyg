using System.Text.Json.Serialization;

namespace Comfyg.Contracts.Changes;

public interface IChangeLog
{
    [JsonIgnore] Type TargetType { get; }

    string TargetId { get; }

    DateTimeOffset ChangedAt { get; }

    ChangeType ChangeType { get; }

    string ChangedBy { get; }
}
