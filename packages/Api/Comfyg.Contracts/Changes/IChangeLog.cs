namespace Comfyg.Contracts.Changes;

public interface IChangeLog
{
    string TargetType { get; }
    
    string TargetId { get; }
    
    DateTime ChangedAt { get; }
    
    ChangeType ChangeType { get; }
    
    string ChangedBy { get; }
}