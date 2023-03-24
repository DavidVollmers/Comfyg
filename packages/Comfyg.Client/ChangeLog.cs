using Comfyg.Contracts.Changes;

namespace Comfyg.Client;

internal class ChangeLog : IChangeLog
{
    public Type TargetType { get; set; } = null!;

    public string TargetId { get; set; } = null!;
    
    public DateTimeOffset ChangedAt { get; set; }
    
    public ChangeType ChangeType { get; set; }

    public string ChangedBy { get; set; } = null!;
}
