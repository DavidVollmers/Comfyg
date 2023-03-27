using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Comfyg.Store.Contracts;

public interface IComfygValue
{
    [Required]
    [MaxLength(1024)]
    [TechnicalIdentifier]
    string Key { get; }

    //TODO define proper max length
    [Required] string Value { get; }

    [ValidateNever] string Version { get; }

    [ValidateNever] DateTimeOffset CreatedAt { get; }

    [JsonIgnore][ValidateNever] string Hash { get; }
}
