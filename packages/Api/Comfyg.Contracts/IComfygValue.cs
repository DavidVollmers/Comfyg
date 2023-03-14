using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Comfyg.Contracts;

public interface IComfygValue
{
    [Required]
    [MaxLength(256)]
    [TechnicalIdentifier]
    string Key { get; }

    [Required] [MaxLength(1024)] string Value { get; }

    [ValidateNever] string Version { get; }
    
    [ValidateNever] DateTime CreatedAt { get; }
}