namespace Comfyg.Contracts.Changes;

internal class ChangeLog : IChangeLog
{
    public Type TargetType { get; set; } = null!;

    public string TargetId { get; set; } = null!;
    
    public DateTimeOffset ChangedAt { get; set; } = DateTimeOffset.UtcNow;
    
    public ChangeType ChangeType { get; set; } = ChangeType.Unknown;
    
    public string ChangedBy { get; set; } = null!;
}