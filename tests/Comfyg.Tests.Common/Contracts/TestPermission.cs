using Comfyg.Store.Core.Abstractions.Permissions;

namespace Comfyg.Tests.Common.Contracts;

public class TestPermission : IPermission
{
    public string Owner { get; set; } = null!;
    
    public string TargetId { get; set; } = null!;
    
    public Type TargetType { get; set; }= null!;
    
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
