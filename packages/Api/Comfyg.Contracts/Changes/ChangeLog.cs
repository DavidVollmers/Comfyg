namespace Comfyg.Contracts.Changes;

internal class ChangeLog : IChangeLog
{
    public string TargetType { get; set; } = null!;
    
    public string TargetId { get; set; } = null!;
    
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    
    public ChangeType ChangeType { get; set; } = ChangeType.Unknown;
    
    public string ChangedBy { get; set; } = null!;
}