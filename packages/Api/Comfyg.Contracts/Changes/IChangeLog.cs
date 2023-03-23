namespace Comfyg.Contracts.Changes;

public interface IChangeLog
{
    Type TargetType { get; }
    
    string TargetId { get; }
    
    DateTimeOffset ChangedAt { get; }
    
    ChangeType ChangeType { get; }
    
    string ChangedBy { get; }
}