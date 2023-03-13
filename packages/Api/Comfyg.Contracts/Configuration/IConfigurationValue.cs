using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Comfyg.Contracts.Configuration;

public interface IConfigurationValue
{
    [Required] [MaxLength(256)] string Key { get; }

    [Required] [MaxLength(1024)] string Value { get; }

    [ValidateNever] string Version { get; }

    [ValidateNever] string[] Tags { get; }
}