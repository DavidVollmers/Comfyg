using System.ComponentModel.DataAnnotations;

namespace Comfyg.Contracts.Configuration;

public interface IConfigurationValue
{
    [Required] [MaxLength(256)] string Key { get; }

    [Required] [MaxLength(1024)] string Value { get; }

    string Version { get; }
    
    string[] Tags { get; }
}