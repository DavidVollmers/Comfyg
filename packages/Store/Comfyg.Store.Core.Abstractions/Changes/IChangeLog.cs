using System.Text.Json.Serialization;

namespace Comfyg.Store.Core.Abstractions.Changes;

public interface IChangeLog
{
    [JsonIgnore] Type TargetType { get; }

    string TargetId { get; }

    DateTimeOffset ChangedAt { get; }

    ChangeType ChangeType { get; }

    string ChangedBy { get; }
}
